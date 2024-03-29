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
    ulong StreamOffset { get; }
    MemoryStream EntryData { get; set; }

    RVBankDataType PackingMethod { get; set; }
    void ExpandDirectoryStructure();
    void InitializeStreamOffset(ulong offset);
    bool InitializeBuffer(BisBinaryReader reader, RVBankOptions options);
    byte[] RetrieveRawBuffer(BisBinaryReader reader, RVBankOptions options);
    public byte[]? RetrieveBuffer(BisBinaryReader reader, RVBankOptions options);
}

public class RVBankDataEntry : RVBankEntry, IRVBankDataEntry, IDisposable
{
    private bool disposed;
    private RVBankDataType packingMethod;
    public ulong StreamOffset { get; protected set; }

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

        var normalizePath = EntryName;

        EntryName = RVPathUtilities.GetFilename(normalizePath);

        var parentName = RVPathUtilities.GetParent(normalizePath);
        if (parentName == "")
        {
            BankFile.PboEntries.Add(this);
            return;
        }
        var parent = BankFile.GetOrCreateDirectory(parentName, BankFile, Logger);

        Move(parent);
    }


    public void InitializeStreamOffset(ulong offset) => StreamOffset = offset;

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
        reader.BaseStream.Seek((long) StreamOffset, SeekOrigin.Begin);
        var dataSize = unchecked((uint)DataSize);
        if (DataSize > int.MaxValue)
        {
            return Array.Empty<byte>();
        }
        var buffer = reader.ReadBytes((int)dataSize);
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

        var writtenSize = BisCompatibleLzss.Compressor.Decode(buffer, out var data, OriginalSize);
        return writtenSize != OriginalSize ? null : data;
    }



    public sealed override Result Binarize(BisBinaryWriter writer, RVBankOptions options) =>
        base.Binarize(writer, options);

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
