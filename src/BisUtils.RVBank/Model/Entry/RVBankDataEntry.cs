namespace BisUtils.RVBank.Model.Entry;

using Alerts.Errors;
using Core.Binarize.Exceptions;
using Core.IO;
using Alerts.Warnings;
using Core.Compression;
using Core.Parsing;
using Enumerations;
using Extensions;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Microsoft.Extensions.Logging;
using Options;
using Stubs;

public interface IRVBankDataEntry : IRVBankEntry
{
    Stream EntryData { get; }
    RVBankDataType PackingMethod { get; set; }
    void ExpandDirectoryStructure();
    bool InitializeData(BisBinaryReader reader, RVBankOptions options);
    public byte[] RetrieveFinalStream(out bool streamWasCompressed);
}

public class RVBankDataEntry : RVBankEntry, IRVBankDataEntry
{

    private RVBankDataType packingMethod;
    public RVBankDataType PackingMethod
    {
        get => packingMethod;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            packingMethod = value;
        }
    }


    private Stream entryData = Stream.Null;
    public Stream EntryData
    {
        get => entryData;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            entryData = value;
        }
    }

    public RVBankDataEntry
    (
        ILogger logger,
        IRVBank file,
        IRVBankDirectory parent,
        string fileName,
        RVBankEntryMime mime,
        uint originalSize,
        uint offset,
        uint timeStamp,
        uint dataSize
    ) : base(fileName, mime, originalSize, offset, timeStamp, dataSize, file, parent, logger)
    {
    }

    public RVBankDataEntry
    (
        string fileName,
        RVBankEntryMime mime,
        uint offset,
        uint timeStamp,
        Stream entryData,
        RVBankDataType? packingMethod,
        IRVBank file,
        IRVBankDirectory parent,
        ILogger? logger
    ) : base(fileName, mime, (uint) entryData.Length, offset, timeStamp, 0, file, parent, logger) =>
        this.packingMethod = packingMethod ?? AssumePackingMethod();

    private RVBankDataType AssumePackingMethod()
    {
        switch (EntryMime)
        {
            case RVBankEntryMime.Encrypted:
                return RVBankDataType.Encrypted;
            case RVBankEntryMime.Decompressed:
                return RVBankDataType.Original;
            default:
                if (OriginalSize == 0 || OriginalSize == DataSize)
                {
                    return RVBankDataType.Original;
                }

                return RVBankDataType.Compressed;
        }
    }

    protected sealed override void OnChangesMade(object? sender, EventArgs? e) => base.OnChangesMade(sender, e);

    public RVBankDataEntry(BisBinaryReader reader, RVBankOptions options, IRVBank file, IRVBankDirectory parent, ILogger? logger) : base(reader, options, file, parent, logger)
    {
        Debinarize(reader, options);
        if (LastResult!.IsFailed)
        {
            throw new DebinarizeFailedException(LastResult.ToString());
        }
    }

    public void SynchronizeMetaWithStream() => OriginalSize = (uint)EntryData.Length;

    public void ExpandDirectoryStructure()
    {
        ArgumentNullException.ThrowIfNull(BankFile, "When expanding a Pbo Entry, The node must be established");

        var normalizePath = EntryName = RVPathUtilities.NormalizePboPath(EntryName);

        if (!EntryName.Contains('\\'))
        {
            return;
        }
        EntryName = RVPathUtilities.GetFilename(EntryName);

        ParentDirectory = BankFile.CreateDirectory(RVPathUtilities.GetParent(normalizePath), BankFile, Logger);
        Move(ParentDirectory);
    }

    public bool InitializeData(BisBinaryReader reader, RVBankOptions options)
    {
        switch (PackingMethod)
        {

            case RVBankDataType.Encrypted:
            {
                throw new RVEncryptedBankException();
            }
            case RVBankDataType.Compressed:
            {
                var start = reader.BaseStream.Position;

                var stream = new MemoryStream();
                using var writer = new BinaryWriter(stream, options.Charset, true);
                if (BisCompatibleLzss.Compressor.Decode(reader, writer, OriginalSize) is { } size && size != OriginalSize)
                {
                    reader.BaseStream.Seek(start + (DataSize - size), SeekOrigin.Begin);
                }
                else
                {
                    entryData = stream;
                    entryData.Seek(0, SeekOrigin.Begin);
                    return true;
                }
                goto default;
            }
            default:
            {

                var memoryStream = new MemoryStream();
                const int BufferSize = 8192;

                var iterations = DataSize / BufferSize;

                for (long i = 0; i < iterations; i++)
                {
                    var chunk = reader.ReadBytes(BufferSize);
                    memoryStream.Write(chunk, 0, chunk.Length);
                }

                var remainder = (int)(DataSize % BufferSize);
                if (remainder > 0)
                {
                    var chunk = reader.ReadBytes(remainder);
                    memoryStream.Write(chunk, 0, chunk.Length);
                }

                entryData = memoryStream;
                entryData.Seek(0, SeekOrigin.Begin);

                return PackingMethod is not RVBankDataType.Compressed;
            }
        }
    }

    public byte[] RetrieveFinalStream(out bool streamWasCompressed)
    {
        switch (PackingMethod)
        {
            case RVBankDataType.Compressed:
            {
                streamWasCompressed = true;
                var stream = BisCompatibleLzss.Compressor.Encode(entryData, out var compressedSize);
                DataSize = compressedSize;
                return stream;
            }
            default:
            {
                streamWasCompressed = false;
                using var data = new MemoryStream();
                entryData.CopyTo(data);
                return data.ToArray();
            }
        }

    }

    public sealed override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        writer.Write(Path);
        writer.Write((long)EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);
        PackingMethod = AssumePackingMethod();
        return LastResult = Result.Ok();
    }

    internal void SetEntryDataQuietly(Stream data) => entryData = data;


    public sealed override Result Validate(RVBankOptions options)
    {
        LastResult = Result.Ok();

        switch (EntryMime)
        {
            case RVBankEntryMime.Encrypted:
                LastResult.WithWarning(new RVBankEncryptedEntryWarning(!options.AllowEncrypted));
                break;
            case RVBankEntryMime.Version:
                LastResult.WithWarning(new RVBankImproperMimeWarning(!options.AllowVersionMimeOnData,
                    typeof(IRVBankDataEntry)));
                break;
            case RVBankEntryMime.Decompressed:
            case RVBankEntryMime.Compressed:
            default:
                break;
        }

        if (EntryName.Length == 0)
        {
            LastResult.WithWarning(new RVBankUnnamedEntryWarning(!options.AllowUnnamedDataEntries, typeof(IRVBankDataEntry)));
        }

        if (EntryData.Length != DataSize && options.CurrentSection != RVBankSection.Header)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IRVBankDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect Stream/DataSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        if (DataSize <= 0)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IRVBankDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect DataSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        if (OriginalSize <= 0)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IRVBankDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect OriginalSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        return LastResult;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        LastResult = base.Debinarize(reader, options);
        EntryMime = (RVBankEntryMime)reader.ReadInt32(); // TODO WARN/ERROR then recover
        OriginalSize = reader.ReadUInt32();
        TimeStamp = reader.ReadUInt32();
        Offset = reader.ReadUInt32();
        DataSize = reader.ReadUInt32();
        packingMethod = AssumePackingMethod();

        return LastResult;
    }

    public override uint CalculateLength(RVBankOptions options) =>  21 + (uint) options.Charset.GetByteCount(Path);
}
