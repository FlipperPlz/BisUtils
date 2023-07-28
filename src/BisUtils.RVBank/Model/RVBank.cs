namespace BisUtils.RVBank.Model;

using System.Collections.ObjectModel;
using System.Security.Cryptography;
using Core.Binarize.Synchronization;
using Core.Extensions;
using Core.IO;
using Enumerations;
using Entry;
using Extensions;
using Stubs;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Microsoft.Extensions.Logging;
using Misc;
using Options;

public interface IRVBank : IBisSynchronizable<RVBankOptions>, IRVBankDirectory
{
    public string FileName { get; set; }

    public string BankPrefix { get; set; }

    string IRVBankEntry.Path => "";

    string IRVBankEntry.AbsolutePath => BankPrefix;

    IRVBankDirectory IRVBankEntry.ParentDirectory
    {
        get => this;
        set => throw new NotSupportedException();
    }

    string IRVBankEntry.EntryName
    {
        get => BankPrefix;
        set => BankPrefix = value;
    }

    RVBankEntryMime IRVBankEntry.EntryMime
    {
        get => RVBankEntryMime.Decompressed;
        set => throw new NotSupportedException();
    }

    int IRVBankEntry.OriginalSize
    {
        get => PboEntries.Sum(it => it.OriginalSize);
        set => throw new NotSupportedException();
    }

    int IRVBankEntry.Offset
    {
        get => 0;
        set => throw new NotSupportedException();
    }

    int IRVBankEntry.TimeStamp
    {
        get => PboEntries.Max(it => it.TimeStamp);
        set => throw new NotSupportedException();
    }

    int IRVBankEntry.DataSize
    {
        get => PboEntries.Sum(it => it.DataSize);
        set => throw new NotSupportedException();
    }

    void IRVBankEntry.Move(IRVBankDirectory directory) =>
        throw new NotSupportedException();

    IEnumerable<IRVBankEntry> IRVBankEntry.MoveAndReplace(IRVBankDirectory directory) => throw new NotSupportedException();

    void IRVBankEntry.Delete() =>
        throw new NotSupportedException();
}

public class RVBank : BisSynchronizable<RVBankOptions>, IRVBank
{
    public string FileName { get; set; }

    public string BankPrefix
    {
        get => this.GetVersionEntry()?.GetPropertyValue("prefix") ?? FileName;
        set => (this.GetVersionEntry() ?? this.AddVersionEntry(Logger)).SetOrCreateProperty("prefix", value, Logger);
    }
    public IRVBank BankFile { get; }

    public bool IsFirstRead { get; private set; } = true;

    private readonly ObservableCollection<IRVBankEntry> pboEntries = null!;
    public ObservableCollection<IRVBankEntry> PboEntries
    {
        get => pboEntries;
        init
        {
            pboEntries = value;
            pboEntries.CollectionChanged += (_, args) =>
            {
                OnChangesMade(this, args);
            };
        }
    }

    public RVBank(string filename, IEnumerable<IRVBankEntry> entries, Stream? synchronizeTo, ILogger? logger) : base(synchronizeTo, logger)
    {
        BankFile = this;
        FileName = filename;
        PboEntries = new ObservableCollection<IRVBankEntry>(entries);
    }

    public RVBank(string filename, BisBinaryReader reader, RVBankOptions options, Stream? synchronizeTo, ILogger? logger) : base(reader, options, synchronizeTo, logger)
    {
        BankFile = this;
        FileName = filename;
        PboEntries = new ObservableCollection<IRVBankEntry>();
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public static RVBank ReadPbo(string path, RVBankOptions options, Stream? syncTo, ILogger logger)
    {
        using var reader = new BisBinaryReader(File.OpenRead(path));
        return new RVBank(Path.GetFileNameWithoutExtension(path), reader, options, syncTo, logger);
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        if(pboEntries.Count != 0)
        {
            PboEntries.Clear();
        }

        var responses = new List<Result>();
        var markedForRemoval = new List<IRVBankEntry>();
        var entries = new List<RVBankDataEntry>();
        Logger?.LogDebug("Entry loop started at {Start}", reader.BaseStream.Position);

        var first = true;
        do
        {

            var start = reader.BaseStream.Position;

            responses.Add(reader.SkipAsciiZ(options));
            var mime = (RVBankEntryMime?)reader.ReadInt64();
            reader.BaseStream.Seek(start, SeekOrigin.Begin);
            IRVBankEntry currentEntry =  mime switch
            {
                RVBankEntryMime.Version => new RVBankVersionEntry(reader, options, this, this, Logger),
                _ => new RVBankDataEntry(reader, options, this, this, Logger)
            };


            if (currentEntry is RVBankDataEntry dataEntry )
            {

                if(currentEntry.IsDummyEntry())
                {
                    if (options.AlwaysSeparateOnDummy)
                    {
                        Logger?.LogDebug("Located ending magic at {Pos}... breaking entry loop.", reader.BaseStream.Position);
                        break;
                    }

                    Logger?.LogDebug("Located ending magic. Peeking for extra data and attempting to recover...");
                    throw new NotImplementedException();
                }

                entries.Add(dataEntry);
                if (!options.FlatRead)
                {
                    dataEntry.ExpandDirectoryStructure();
                }

                markedForRemoval.AddRange(dataEntry.RetrieveDuplicateEntries());
            }
            else
            {
                pboEntries.Add(currentEntry);

            }

            var response = currentEntry.LastResult ?? Result.Fail("Unknown Error Occured");

            if (first && currentEntry is not IRVBankVersionEntry)
            {
                Logger?.LogInformation("First entry is not a version entry... this is weird");

                response.WithWarning(new Warning
                {
                    Message = "The first entry in a PBO should always be a version entry.",
                    AlertName = "FirstIsNotVersion",
                    AlertScope = typeof(RVBank),
                    IsError = options.RequireFirstEntryIsVersion
                });
            }

            if (first)
            {
                first = false;
            }

            responses.Add(response);


        } while (true);

        Logger?.LogDebug("Entry loop ended at {Pos} with {Count} entry(s) found and {DupesCount} duplicate entries, starting data sweep", reader.BaseStream.Position, pboEntries.Count, markedForRemoval.Count);
        var headerEnd = reader.BaseStream.Position;
        var end = headerEnd;
        foreach (var entry in entries)
        {

            Logger?.LogDebug("{End}", entry.DataSize);

            if (end <= headerEnd || end >= reader.BaseStream.Length )
            {
                goto Ignore;
            }
            entry.InitializeStreamOffset(end);

            if (
                entry.InitializeBuffer(reader, options))
            {

                end += entry.DataSize;
                continue;
            }
            Ignore:

            end += entry.DataSize;
            if (entry.EntryMime is RVBankEntryMime.Encrypted)
            {
                Logger?.LogInformation("Ignoring Encrypted entry '{EntryName}' at '{Position}' with length '{Length}", entry.AbsolutePath, end, entry.DataSize);

            }
            if (entry.EntryName.Contains("config.cpp"))
            {
                Logger?.LogInformation("Ignoring malformed entry '{EntryName}' at '{Position}' with length '{Length}", entry.AbsolutePath, end, entry.DataSize);

            }

            markedForRemoval.Add(entry);
            continue;
        }

        Logger?.LogDebug("Data sweep ended at {Pos}. {IgnoreCount} entry(s) were skipped to avoid the extraction of unused data.", reader.BaseStream.Position, markedForRemoval.Count);

        Logger?.LogDebug("Removing all {IgnoreCount} deleted/stale entries from the directory structure.", markedForRemoval.Count);
        markedForRemoval.ForEach(it => it.Delete());
        var positionToRead = Math.Max(0, reader.BaseStream.Length - 21);
        reader.BaseStream.Seek(positionToRead, SeekOrigin.Begin);

        var calculatedDigest = CalculateDigest(reader.BaseStream);
        var writtenDigest = ReadDigest(reader);
        if (writtenDigest != calculatedDigest)
        {
            //TODO: Log and warn that invalid checksum was found
        }

        End:
        {
            LastResult = Result.Merge(responses);

            if (IsFirstRead)
            {
                IsFirstRead = false;
            }

            return LastResult;
        }
    }

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        // writer.BaseStream.Seek(pboEntries.Sum(it => it.CalculateLength(options)), SeekOrigin.Begin);
        // writer.Write(new byte[21]);
        // foreach (var entry in this.GetDataEntries(SearchOption.AllDirectories))
        // {
        //     writer.Write(entry.RetrieveFinalStream(out var compressed));
        // }
        // var dataEnd = writer.BaseStream.Position;
        // writer.BaseStream.Seek(0, SeekOrigin.Begin);
        // foreach (var entry in pboEntries)
        // {
        //     entry.Binarize(writer, options);
        // }
        // writer.BaseStream.Seek(dataEnd, SeekOrigin.Begin);
        // writer.Write((byte)0);
        // var digest = CalculateDigest(writer.BaseStream);
        // digest.Write(writer);
        return LastResult!;
    }

    public override Result Validate(RVBankOptions options) => throw new NotImplementedException();

    private static RVBankDigest ReadDigest(BisBinaryReader reader) =>
        new(reader);

#pragma warning disable SYSLIB0021
#pragma warning disable CA5350
    private static RVBankDigest CalculateDigest(Stream stream)
    {
        var oldPosition = stream.Position;
        stream.Seek(0, SeekOrigin.Begin);


        using var alg = new SHA1Managed();
        var digest = new RVBankDigest(alg.ComputeHash(stream));

        stream.Seek(oldPosition, SeekOrigin.Begin);

        return digest;
    }
#pragma warning restore CA5350
#pragma warning restore SYSLIB0021
    // public int CalculateLength(RVBankOptions options) => pboEntries.Sum(it => it.CalculateLength(options) + it.DataSize) + 20;

    public int CalculateHeaderLength(RVBankOptions options) => throw new NotImplementedException();
}
