namespace BisUtils.RVBank.Model;

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

    uint IRVBankEntry.OriginalSize
    {
        get => PboEntries.UnsignedSum(it => it.OriginalSize);
        set => throw new NotSupportedException();
    }

    ulong IRVBankEntry.Offset
    {
        get => 0;
        set => throw new NotSupportedException();
    }

    ulong IRVBankEntry.TimeStamp
    {
        get => PboEntries.Max(it => it.TimeStamp);
        set => throw new NotSupportedException();
    }

    ulong IRVBankEntry.DataSize
    {
        get => PboEntries.UnsignedSum(it => it.DataSize);
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

    protected RVBank(IEnumerable<IRVBankEntry>? entries, string filename, Stream? syncTo, ILogger? logger) : base(syncTo, logger)
    {
        BankFile = this;
        FileName = filename;
        PboEntries = entries != null ? new ObservableCollection<IRVBankEntry>(entries) : new ObservableCollection<IRVBankEntry>();
    }



    public RVBank(string filename, BisBinaryReader reader, RVBankOptions options, Stream? syncTo, ILogger? logger) : base(reader, options, syncTo, logger)
    {
        BankFile = this;
        FileName = filename;
        PboEntries = new ObservableCollection<IRVBankEntry>();
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RVBank(string fileName, Stream buffer, RVBankOptions options, Stream? syncTo, ILogger logger) : this(null, fileName, syncTo, logger)
    {
        using var reader = new BisBinaryReader(buffer, options.Charset);
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RVBank(string path, RVBankOptions options, Stream? syncTo, ILogger logger) : this(null, Path.GetFileNameWithoutExtension(path), syncTo, logger)
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
        out List<IRVBankDataEntry> dataEntries,
        out List<IRVBankEntry> queuedForRemoval,
        ICollection<IRVBankEntry> directoryStructure,
        IRVBank parent,
        ILogger? logger,
        BisBinaryReader reader,
        RVBankOptions options
    )
    {
        var fileCount = 0;
        bufferEnd = 0;
        headerStart = Convert.ToInt32(reader.BaseStream.Position);
        queuedForRemoval = new List<IRVBankEntry>();
        dataEntries = new List<IRVBankDataEntry>();
        logger?.LogDebug("Entry loop started at {Start}", headerStart);
        for (;;)
        {
            ReadBankInfo(out var entry,parent, logger, reader, options);

            if(entry.IsDummyEntry() && entry is not RVBankVersionEntry)
            {
                break;
            }
            fileCount++;
            directoryStructure.Add(entry);

            if (entry is IRVBankDataEntry dataEntry)
            {
                dataEntries.Add(dataEntry);
                dataEntry.InitializeStreamOffset((uint) bufferEnd);
                if (!options.FlatRead)
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


    public static bool ReadBankInfo(out IRVBankEntry entry, IRVBank parent, ILogger? logger, BisBinaryReader reader, RVBankOptions options) {
        var start = reader.BaseStream.Position;

        reader.SkipAsciiZ(options);
        var mime = (RVBankEntryMime?)reader.ReadInt64();
        reader.BaseStream.Seek(start, SeekOrigin.Begin);
        entry = mime switch
        {
            RVBankEntryMime.Version => new RVBankVersionEntry(reader, options, parent, parent, logger),
            _ => new RVBankDataEntry(reader, options, parent, parent, logger)
        };
        return entry is IRVBankVersionEntry;
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

    private void CalculateEntryOffsets(List<IRVBankDataEntry> dataEntries, ICollection<IRVBankEntry> queuedForRemoval, uint headerEnd, BisBinaryReader reader, RVBankOptions options)
    {
        Logger?.LogInformation("Starting data sweep at {Pos}.", reader.BaseStream.Position);
        foreach (var entry in dataEntries)
        {
            bool shouldRemove;
            var entryOffset = entry.StreamOffset + headerEnd;
            entry.InitializeStreamOffset(entryOffset);
            if ( entryOffset < headerEnd )
            {
                shouldRemove = true;
                goto Continue;
            }

            var entryEnd =  entry.StreamOffset + entry.DataSize;
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



    //TODO(bank): Binarize
    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options) => LastResult!;

    //TODO(bank): Validate
    public override Result Validate(RVBankOptions options) => throw new NotImplementedException();

    //TODO(bank): CalculateHeaderLength
    public int CalculateHeaderLength(RVBankOptions options) => throw new NotImplementedException();
}
