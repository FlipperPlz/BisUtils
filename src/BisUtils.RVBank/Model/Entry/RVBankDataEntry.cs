namespace BisUtils.RVBank.Model.Entry;

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
    long StreamOffset { get; }
    MemoryStream EntryData { get; set; }

    RVBankDataType PackingMethod { get; set; }
    void ExpandDirectoryStructure();
    void InitializeStreamOffset(long offset);
    bool InitializeBuffer(BisBinaryReader reader, RVBankOptions options);
    byte[] RetrieveRawBuffer(BisBinaryReader reader, RVBankOptions options);
    public byte[]? RetrieveBuffer(BisBinaryReader reader, RVBankOptions options);
}

public class RVBankDataEntry : RVBankEntry, IRVBankDataEntry, IDisposable
{
    private bool disposed;
    private RVBankDataType packingMethod;
    public long StreamOffset { get; protected set; }

    private MemoryStream entryData = null!;
    public MemoryStream EntryData
    {
        get => entryData;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            entryData.Dispose();
            entryData = value;
        }
    }

    public RVBankDataType PackingMethod
    {
        get => packingMethod;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            packingMethod = value;
        }
    }

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


    public RVBankDataEntry(BisBinaryReader reader, RVBankOptions options, IRVBank file, IRVBankDirectory parent, ILogger? logger) : base(reader, options, file, parent, logger)
    {
        Debinarize(reader, options);
        if (LastResult!.IsFailed)
        {
            throw new DebinarizeFailedException(LastResult.ToString());
        }
    }

    public void ExpandDirectoryStructure()
    {
        ArgumentNullException.ThrowIfNull(BankFile, "When expanding a Pbo Entry, The node must be established");

        var normalizePath = EntryName = RVPathUtilities.NormalizePboPath(EntryName);

        if (!EntryName.Contains('\\'))
        {
            return;
        }
        EntryName = RVPathUtilities.GetFilename(EntryName);

        Move(BankFile.GetOrCreateDirectory(RVPathUtilities.GetParent(normalizePath), BankFile, Logger));
    }



    public void InitializeStreamOffset(long offset) => StreamOffset = offset;

    public bool InitializeBuffer(BisBinaryReader reader, RVBankOptions options)
    {
        if (RetrieveBuffer(reader, options) is not { } buffer)
        {
            return false;
        }
        entryData = new MemoryStream(buffer, false);
        return true;
    }

    public byte[] RetrieveRawBuffer(BisBinaryReader reader, RVBankOptions options)
    {
        var start = reader.BaseStream.Position;
        reader.BaseStream.Seek(StreamOffset, SeekOrigin.Begin);
        var buffer = reader.ReadBytes(DataSize);
        reader.BaseStream.Seek(start, SeekOrigin.Begin);
        return buffer;
    }

    public virtual byte[]? RetrieveBuffer(BisBinaryReader reader, RVBankOptions options)
    {
        var buffer = RetrieveRawBuffer(reader, options);
        if (PackingMethod is not RVBankDataType.Compressed)
        {
            return buffer;
        }

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, options.Charset, true);

        if (!(BisCompatibleLzss.Compressor.Decode(buffer, writer, OriginalSize) is { } size && size != OriginalSize))
        {
            return null;
        }

        return buffer;
    }



    public sealed override Result Binarize(BisBinaryWriter writer, RVBankOptions options) => base.Binarize(writer, options);

    public sealed override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        var result = base.Debinarize(reader, options);
        packingMethod = AssumePackingMethod();
        return result;
    }

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

    public sealed override int CalculateHeaderLength(RVBankOptions options) => base.CalculateHeaderLength(options);

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        entryData.Dispose();
        GC.SuppressFinalize(this);
        disposed = true;
    }
}
