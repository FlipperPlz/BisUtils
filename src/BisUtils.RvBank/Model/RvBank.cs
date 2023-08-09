namespace BisUtils.RvBank.Model;

using System.Collections.ObjectModel;
using Alerts.Exceptions;
using Core.Binarize.Synchronization;
using Core.Extensions;
using Core.IO;
using Enumerations;
using Entry;
using Extensions;
using Stubs;
using FResults;
using Microsoft.Extensions.Logging;
using Misc;
using Options;

public interface IRvBank : IBisSynchronizable<RvBankOptions>, IRvBankDirectory
{
    public string FileName { get; set; }

    public string BankPrefix { get; set; }

    string IRvBankEntry.Path => "";

    string IRvBankEntry.AbsolutePath => BankPrefix;

    IRvBankDirectory IRvBankEntry.ParentDirectory
    {
        get => this;
        set => throw new NotSupportedException();
    }

    string IRvBankEntry.EntryName
    {
        get => BankPrefix;
        set => BankPrefix = value;
    }

    RvBankEntryMime IRvBankEntry.EntryMime
    {
        get => RvBankEntryMime.Decompressed;
        set => throw new NotSupportedException();
    }

    uint IRvBankEntry.OriginalSize
    {
        get => PboEntries.UnsignedSum(it => it.OriginalSize);
        set => throw new NotSupportedException();
    }

    ulong IRvBankEntry.Offset
    {
        get => 0;
        set => throw new NotSupportedException();
    }

    ulong IRvBankEntry.TimeStamp
    {
        get => PboEntries.Max(it => it.TimeStamp);
        set => throw new NotSupportedException();
    }

    ulong IRvBankEntry.DataSize
    {
        get => PboEntries.UnsignedSum(it => it.DataSize);
        set => throw new NotSupportedException();
    }

    void IRvBankEntry.Move(IRvBankDirectory directory) =>
        throw new NotSupportedException();

    IEnumerable<IRvBankEntry> IRvBankEntry.MoveAndReplace(IRvBankDirectory directory) => throw new NotSupportedException();

    void IRvBankEntry.Delete() =>
        throw new NotSupportedException();
}

public class RvBank : BisSynchronizable<RvBankOptions>, IRvBank
{
    public string FileName { get; set; }

    public string BankPrefix
    {
        get => this.GetVersionEntry()?.GetPropertyValue("prefix") ?? FileName;
        set => (this.GetVersionEntry() ?? this.AddVersionEntry(Logger)).SetOrCreateProperty("prefix", value, Logger);
    }
    public IRvBank BankFile { get; }

    public bool IsFirstRead { get; private set; } = true;

    private readonly ObservableCollection<IRvBankEntry> pboEntries = null!;
    public ObservableCollection<IRvBankEntry> PboEntries
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

    protected RvBank(IEnumerable<IRvBankEntry>? entries, string filename, Stream? syncTo, ILogger? logger) : base(syncTo, logger)
    {
        BankFile = this;
        FileName = filename;
        PboEntries = entries != null ? new ObservableCollection<IRvBankEntry>(entries) : new ObservableCollection<IRvBankEntry>();
    }



    public RvBank(string filename, BisBinaryReader reader, RvBankOptions options, Stream? syncTo, ILogger? logger) : base(reader, options, syncTo, logger)
    {
        BankFile = this;
        FileName = filename;
        PboEntries = new ObservableCollection<IRvBankEntry>();
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RvBank(string fileName, Stream buffer, RvBankOptions options, Stream? syncTo, ILogger logger) : this(null, fileName, syncTo, logger)
    {
        using var reader = new BisBinaryReader(buffer, options.Charset);
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RvBank(string path, RvBankOptions options, Stream? syncTo, ILogger logger) : this(null, Path.GetFileNameWithoutExtension(path), syncTo, logger)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BisBinaryReader(stream, options.Charset);
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public static int ReadBankHeader
    (
        long bankLength,
        out int bufferEnd,
        out int headerLength,
        out int headerEnd,
        out int headerStart,
        out List<IRvBankDataEntry> dataEntries,
        out List<IRvBankEntry> queuedForRemoval,
        ICollection<IRvBankEntry> directoryStructure,
        IRvBank parent,
        ILogger? logger,
        BisBinaryReader reader,
        RvBankOptions options
    )
    {
        var fileCount = 0;
        bufferEnd = 0;
        headerStart = Convert.ToInt32(reader.BaseStream.Position);
        queuedForRemoval = new List<IRvBankEntry>();
        dataEntries = new List<IRvBankDataEntry>();
        logger?.LogDebug("Entry loop started at {Start}", headerStart);
        for (;;)
        {
            ReadBankInfo(out var entry,parent, logger, reader, options);

            if(entry.IsDummyEntry() && entry is not RvBankVersionEntry)
            {
                break;
            }
            fileCount++;
            directoryStructure.Add(entry);

            if (entry is IRvBankDataEntry dataEntry)
            {
                dataEntries.Add(dataEntry);
                dataEntry.InitializeStreamOffset((uint) bufferEnd);
                if (!options.FlatRead && !queuedForRemoval.Contains(dataEntry) && dataEntry.DataSize != 0)
                {
                    dataEntry.ExpandDirectoryStructure();
                }

            }

            bufferEnd += (int) entry.DataSize;
        }
        headerEnd = Convert.ToInt32(reader.BaseStream.Position);
        headerLength = headerEnd - headerStart;
        bufferEnd += headerLength;
        if (bufferEnd > bankLength)
        {
            throw new OverflowException(
                $"Bank file supplied is to short to contain data for the files written in its dictionary. Need at least {bufferEnd} bytes, instead found {bankLength}!");
        }
        logger?.LogInformation("Entry loop ended at {Pos} with {Count} entry(s) found and {DupesCount} duplicate entries, starting data sweep", headerEnd, fileCount, queuedForRemoval.Count);
        return fileCount;
    }

    private static bool ShouldRemoveEntry(IRvBankDataEntry entry, uint headerLength)
    {
        var entryOffset = entry.StreamOffset + headerLength;

        if (entryOffset < headerLength)
        {
            return true;
        }

        var entryEnd = entry.StreamOffset + entry.DataSize;

        if (!(entry.StreamOffset >= headerLength && entryEnd >= headerLength))
        {
            return true;
        }

        return entry.EntryMime is RvBankEntryMime.Encrypted;
    }


    public static bool ReadBankInfo(out IRvBankEntry entry, IRvBank parent, ILogger? logger, BisBinaryReader reader, RvBankOptions options) {
        var start = reader.BaseStream.Position;

        reader.SkipAsciiZ(options);
        var mime = (RvBankEntryMime?)reader.ReadInt64();
        reader.BaseStream.Seek(start, SeekOrigin.Begin);
        entry = mime switch
        {
            RvBankEntryMime.Version => new RvBankVersionEntry(reader, options, parent, parent, logger),
            _ => new RvBankDataEntry(reader, options, parent, parent, logger)
        };
        return entry is IRvBankVersionEntry;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RvBankOptions options)
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
            PboEntries,
            this,
            Logger,
            reader,
            options
        );
        CalculateEntryOffsets(dataEntries, queuedForRemoval, unchecked((uint) headerEnd), reader, options);
        queuedForRemoval.ForEach(it => it.Delete());
        ReadDigest(bufferEnd, reader, options);

        return LastResult;
    }

    private void CalculateEntryOffsets(List<IRvBankDataEntry> dataEntries, ICollection<IRvBankEntry> queuedForRemoval, uint headerEnd, BisBinaryReader reader, RvBankOptions options)
    {
        Logger?.LogInformation("Starting data sweep at {Pos}.", reader.BaseStream.Position);
        foreach (var entry in dataEntries)
        {
            var entryOffset = entry.StreamOffset + headerEnd;
            entry.InitializeStreamOffset(entryOffset);

            if (ShouldRemoveEntry(entry, headerEnd))
            {
                Logger?.LogDebug("Ignoring encrypted or malformed entry '{EntryName}' at '{Position}' with length '{Length}", entry.AbsolutePath, entry.StreamOffset, entry.DataSize);
                queuedForRemoval.Add(entry);
            }
            else if (!entry.InitializeBuffer(reader, options))
            {
                Logger?.LogDebug("Failed to read entry '{EntryName}' at '{Position}' with length '{Length}", entry.AbsolutePath, entry.StreamOffset, entry.DataSize);
                queuedForRemoval.Add(entry);
            }
        }
        Logger?.LogInformation("Data sweep ended at {Pos}. {IgnoreCount} entry(s) were skipped to avoid the extraction of unused and/or malformed data.", reader.BaseStream.Position, queuedForRemoval.Count);
    }


    private void ReadDigest(int bufferEnd, BisBinaryReader reader, RvBankOptions options)
    {
        reader.BaseStream.Seek(bufferEnd + 1, SeekOrigin.Begin);

        var calculatedDigest = RvBankDigest.CalculateStreamDigest(reader.BaseStream, true);
        var writtenDigest = new RvBankDigest(reader);
        if (writtenDigest == calculatedDigest)
        {
            return;
        }

        Logger?.LogWarning("The checksum in this pbo appears to be incorrect. Expected '{ActualDigest}', but instead got '{WrittenDigest}'.", Convert.ToBase64String(calculatedDigest.ToByteArray()), Convert.ToBase64String(writtenDigest.ToByteArray()));
        if (options.RequireValidSignature)
        {
            throw new RvBankChecksumMismatchException(calculatedDigest, writtenDigest);
        }
    }



    //TODO(bank): Binarize
    public override Result Binarize(BisBinaryWriter writer, RvBankOptions options) => LastResult!;

    //TODO(bank): Validate
    public override Result Validate(RvBankOptions options) => throw new NotImplementedException();

    //TODO(bank): CalculateHeaderLength
    public int CalculateHeaderLength(RvBankOptions options) =>
        PboEntries.Sum(s => s.CalculateHeaderLength(options)) + 21;
}
