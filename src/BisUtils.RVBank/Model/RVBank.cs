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

    IRVBankDirectory IRVBankVfsEntry.ParentDirectory => this;

    string IRVBankVfsEntry.Path => "";

    string IRVBankVfsEntry.AbsolutePath => BankPrefix;

    string IRVBankVfsEntry.EntryName
    {
        get => BankPrefix;
        set => BankPrefix = value;
    }

}

public class RVBank : BisSynchronizable<RVBankOptions>, IRVBank
{
    public string FileName { get; set; }

    public string BankPrefix
    {
        get => this.GetVersionEntry()?.GetPropertyValue("prefix") ?? FileName;
        set => (this.GetVersionEntry() ?? this.AddVersionEntry(Logger)).SetOrCreateProperty("prefix", value);
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
        Logger?.LogInformation("Debinarization has started on bank {BankName}", FileName);

        var responses = new List<Result>();
        var markedForRemoval = new List<IRVBankEntry>();
        Logger?.LogDebug("Entry loop started at {Start}", reader.BaseStream.Position);

        var first = true;
        do
        {

            var start = reader.BaseStream.Position;

            responses.Add(reader.SkipAsciiZ(options));
            var mime = (RVBankEntryMime?)reader.ReadInt64();
            reader.BaseStream.Seek(start, SeekOrigin.Begin);
            IRVBankEntry currentEntry = mime switch
            {
                RVBankEntryMime.Version => new RVBankVersionEntry(reader, options, this, this, Logger),
                _ => new RVBankDataEntry(reader, options, this, this, Logger)
            };


            if (currentEntry is RVBankDataEntry dataEntry)
            {
                if (!options.FlatRead)
                {
                    dataEntry.ExpandDirectoryStructure();
                }
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

            if (currentEntry is IRVBankDataEntry && currentEntry.IsDummyEntry())
            {
                if (options.AlwaysSeparateOnDummy)
                {
                    Logger?.LogDebug("Located ending magic at {Pos}... breaking entry loop.", reader.BaseStream.Position);
                    break;
                }

                Logger?.LogDebug("Located ending magic. Peeking for extra data and attempting to recover...");
            }

            if (first)
            {
                first = false;
            }

            responses.Add(response);
            if (currentEntry.ParentDirectory == this)
            {
                PboEntries.Add(currentEntry);
            }

            if (options.IgnoreDuplicateFiles && currentEntry.RetrieveDuplicateEntry() is { } removeThis)
            {
                markedForRemoval.Add(removeThis);
            }
        } while (true);

        Logger?.LogDebug("Entry loop ended at {Pos} with {Count} entry(s) found and {DupesCount} duplicate entries, starting data sweep", reader.BaseStream.Position, pboEntries.Count, markedForRemoval.Count);

        foreach (var entry in this.GetFileEntries())
        {
            if (markedForRemoval.Contains(entry))
            {
                reader.BaseStream.Seek(entry.DataSize, SeekOrigin.Current);
                continue;
            }


            if (!entry.InitializeData(reader, options))
            {
                markedForRemoval.Add(entry);
            }
        }
        Logger?.LogDebug("Data sweep ended at {Pos}. {IgnoreCount} entry(s) were skipped to avoid the extraction of unused data.", reader.BaseStream.Position, markedForRemoval.Count);

        Logger?.LogDebug("Removing deleted/stale entries from the directory structure.");

        foreach (var removable in markedForRemoval)
        {
            removable.ParentDirectory.RemoveEntry(removable);
        }

        var remaining = reader.BaseStream.Length - reader.BaseStream.Position;

        switch (remaining)
        {
            case < 20:
                //TODO: Log and warn that a full checksum was not found; reading is finished
                goto End;
            case >= 20:
                //TODO: Log and warn that extra data was found
                break;
        }

        var calculatedDigest = CalculateDigest(reader.BaseStream);
        var writtenDigest = ReadDigest(reader);
        if (writtenDigest != calculatedDigest)
        {
            //TODO: Log and warn that invalid checksum was found
        }

        End:
        {
            LastResult = Result.Merge(responses);
            if (reader.BaseStream == SynchronizationStream)
            {
                OnChangesSaved(EventArgs.Empty);
            }


            if (IsFirstRead)
            {
                IsFirstRead = false;
            }
            Logger?.LogInformation("Debinarization has finished on bank {BankName}", FileName);

            return LastResult;
        }
    }

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        var headerStart = writer.BaseStream.Position;
        var headerLength = PboEntries.Sum(it => it.CalculateLength(options));
        //var dataStart = writer.BaseStream.Position;
        //long dataLength = 0;
        writer.BaseStream.Seek(headerLength, SeekOrigin.Current);
        foreach (var entry in this.GetFileEntries(true))
        {
            var data = entry.RetrieveFinalStream(out var compressed);
            //dataLength += data.Length;
            writer.Write(data);
            if (compressed)
            {
                data.Close();
            }
        }
        var dataEnd = writer.BaseStream.Position;
        writer.BaseStream.Seek(headerStart, SeekOrigin.Begin);
        LastResult = Result.Ok().WithReasons(PboEntries.SelectMany(it => it.Binarize(writer, options).Reasons));
        writer.BaseStream.Seek(dataEnd, SeekOrigin.Begin);
        var digest = CalculateDigest(writer.BaseStream);
        digest.Write(writer);
        return LastResult;
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
    public uint CalculateLength(RVBankOptions options) => (uint) pboEntries.Sum(it => it.CalculateLength(options) + it.DataSize) + 20;
}
