namespace BisUtils.RVBank.Model;

using System.Collections.ObjectModel;
using System.Security.Cryptography;
using Alerts.Exceptions;
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
        using var stream = File.OpenRead(path);
        using var reader = new BisBinaryReader(stream, options.Charset);
        return new RVBank(Path.GetFileNameWithoutExtension(path), reader, options, syncTo, logger);
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        if(pboEntries.Count != 0)
        {
            PboEntries.Clear();
        }

        LastResult = Result.Ok();
        var bankLength = reader.BaseStream.Length;
        ReadBankHeader
        (
            bankLength,
            out var bufferEnd,
            out _,
            out var headerEnd,
            out _,
            out var dataEntries,
            out var queuedForRemoval,
            reader,
            options
        );
        CalculateEntryOffsets(dataEntries, queuedForRemoval, headerEnd, reader, options);
        //queuedForRemoval.ForEach(it => it.Delete());
        ReadDigest(bufferEnd, reader, options);

        return LastResult;
    }

    private void CalculateEntryOffsets(List<IRVBankDataEntry> dataEntries, ICollection<IRVBankEntry> queuedForRemoval, int headerEnd, BisBinaryReader reader, RVBankOptions options)
    {
        Logger?.LogInformation("Starting data sweep at {Pos}.", reader.BaseStream.Position);
        foreach (var entry in dataEntries)
        {
            var shouldRemove = false;
            var entryOffset =  entry.StreamOffset + headerEnd;
            entry.InitializeStreamOffset(entryOffset);
            Console.WriteLine(entry.StreamOffset + " " + entry.DataSize);
            if ( entryOffset < headerEnd )
            {
                shouldRemove = true;
                goto Continue;
            }
            var entryEnd = entry.StreamOffset + entry.DataSize;
            if
            (
                entry.StreamOffset >= headerEnd &&
                entryEnd >= headerEnd &&
                entry.InitializeBuffer(reader, options)
            )
            {
                continue;
            }
            shouldRemove = true;
            Continue:

            if (entry.EntryMime is RVBankEntryMime.Encrypted)
            {
                shouldRemove = true;
            }

            if (!shouldRemove)
            {
                continue;
            }
            Logger?.LogDebug("Ignoring encrypted or malformed entry '{EntryName}' at '{Position}' with length '{Length}", entry.AbsolutePath, entry.StreamOffset, entry.DataSize);

            queuedForRemoval.Add(entry);
        }
        Logger?.LogInformation("Data sweep ended at {Pos}. {IgnoreCount} entry(s) were skipped to avoid the extraction of unused and/or malformed data.", reader.BaseStream.Position, queuedForRemoval.Count);
    }


    private void ReadDigest(int bufferEnd, BisBinaryReader reader, RVBankOptions options)
    {
        reader.BaseStream.Seek(bufferEnd + 1, SeekOrigin.Begin);

        var calculatedDigest = RVBankDigest.CalculateStreamDigest(reader.BaseStream, true);
        var writtenDigest = new RVBankDigest(reader);
        if (writtenDigest == calculatedDigest)
        {
            return;
        }

        Logger?.LogWarning("The checksum in this pbo appears to be incorrect. Expected '{ActualDigest}', but instead got '{WrittenDigest}'.", Convert.ToBase64String(calculatedDigest.ToByteArray()), Convert.ToBase64String(writtenDigest.ToByteArray()));
        if (options.RequireValidSignature)
        {
            throw new RVBankChecksumMismatch(calculatedDigest, writtenDigest);
        }
    }

    private int ReadBankHeader(long bankLength, out int bufferEnd, out int headerLength, out int headerEnd, out int headerStart, out List<IRVBankDataEntry> dataEntries , out List<IRVBankEntry> queuedForRemoval, BisBinaryReader reader, RVBankOptions options)
    {
        var fileCount = 0;
        bufferEnd = 0;
        headerStart = Convert.ToInt32(reader.BaseStream.Position);
        queuedForRemoval = new List<IRVBankEntry>();
        dataEntries = new List<IRVBankDataEntry>();
        Logger?.LogDebug("Entry loop started at {Start}", headerStart);
        for (;;)
        {
            ReadBankInfo(out var entry, reader, options);

            if(entry.IsDummyEntry() && entry is not RVBankVersionEntry)
            {
                break;
            }
            fileCount++;
            PboEntries.Add(entry);

            if (entry is IRVBankDataEntry dataEntry)
            {
                dataEntries.Add(dataEntry);
                dataEntry.InitializeStreamOffset(bufferEnd);
                if (!options.FlatRead)
                {
                    dataEntry.ExpandDirectoryStructure();
                }
            }
            bufferEnd += entry.DataSize;
        }
        headerEnd = Convert.ToInt32(reader.BaseStream.Position);
        headerLength = headerEnd - headerStart;
        bufferEnd += headerLength;
        if (bankLength > bufferEnd)
        {
            throw new OverflowException(
                $"Bank file supplied is to short to contain data for the files written in its dictionary. Need at least {bufferEnd} bytes, instead found {bankLength}!");
        }
        Logger?.LogInformation("Entry loop ended at {Pos} with {Count} entry(s) found and {DupesCount} duplicate entries, starting data sweep", headerEnd, fileCount, queuedForRemoval.Count);
        return fileCount;
    }


    private bool ReadBankInfo(out IRVBankEntry entry, BisBinaryReader reader, RVBankOptions options) {
        var start = reader.BaseStream.Position;

        reader.SkipAsciiZ(options);
        var mime = (RVBankEntryMime?)reader.ReadInt64();
        reader.BaseStream.Seek(start, SeekOrigin.Begin);
        entry = mime switch
        {
            RVBankEntryMime.Version => new RVBankVersionEntry(reader, options, this, this, Logger),
            _ => new RVBankDataEntry(reader, options, this, this, Logger)
        };
        return entry is IRVBankVersionEntry;
    }

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        //writer.BaseStream.Seek(pboEntries.Sum(it => it.CalculateLength(options)), SeekOrigin.Begin);
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

    // public int CalculateLength(RVBankOptions options) => pboEntries.Sum(it => it.CalculateLength(options) + it.DataSize) + 20;

    public int CalculateHeaderLength(RVBankOptions options) => throw new NotImplementedException();
}
