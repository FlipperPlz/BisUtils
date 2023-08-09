namespace BisUtils.RvBank.Model.Entry;

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

public interface IRvBankDataEntry : IRvBankEntry
{
    ulong StreamOffset { get; }
    MemoryStream EntryData { get; set; }

    RvBankDataType PackingMethod { get; set; }
    void ExpandDirectoryStructure();
    void InitializeStreamOffset(ulong offset);
    bool InitializeBuffer(BisBinaryReader reader, RvBankOptions options);
    byte[] RetrieveRawBuffer(BisBinaryReader reader, RvBankOptions options);
    public byte[]? RetrieveBuffer(BisBinaryReader reader, RvBankOptions options);
}

public class RvBankDataEntry : RvBankEntry, IRvBankDataEntry, IDisposable
{
    private bool disposed;
    private RvBankDataType packingMethod;
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

    public RvBankDataType PackingMethod
    {
        get => packingMethod;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            packingMethod = value;
        }
    }

    private RvBankDataType AssumePackingMethod()
    {
        switch (EntryMime)
        {
            case RvBankEntryMime.Encrypted:
                return RvBankDataType.Encrypted;
            case RvBankEntryMime.Decompressed:
                return RvBankDataType.Original;
            default:
                if (OriginalSize == 0 || OriginalSize == DataSize)
                {
                    return RvBankDataType.Original;
                }

                return RvBankDataType.Compressed;
        }
    }


    public RvBankDataEntry(BisBinaryReader reader, RvBankOptions options, IRvBank file, IRvBankDirectory parent, ILogger? logger) : base(reader, options, file, parent, logger)
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

    public bool InitializeBuffer(BisBinaryReader reader, RvBankOptions options)
    {
        if (RetrieveBuffer(reader, options) is not { } buffer)
        {
            return false;
        }
        entryData = new MemoryStream(buffer, false);
        return true;
    }

    public byte[] RetrieveRawBuffer(BisBinaryReader reader, RvBankOptions options)
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

    public virtual byte[]? RetrieveBuffer(BisBinaryReader reader, RvBankOptions options)
    {
        var buffer = RetrieveRawBuffer(reader, options);
        if (PackingMethod is not RvBankDataType.Compressed)
        {
            return buffer;
        }

        var writtenSize = BisCompatibleLzss.Compressor.Decode(buffer, out var data, OriginalSize);
        return writtenSize != OriginalSize ? null : data;
    }



    public sealed override Result Binarize(BisBinaryWriter writer, RvBankOptions options) =>
        base.Binarize(writer, options);

    public sealed override Result Debinarize(BisBinaryReader reader, RvBankOptions options)
    {
        var result = base.Debinarize(reader, options);
        packingMethod = AssumePackingMethod();
        return result;
    }

    public sealed override Result Validate(RvBankOptions options)
    {
        LastResult = Result.Ok();

        switch (EntryMime)
        {
            case RvBankEntryMime.Encrypted:
                LastResult.WithWarning(new RvBankEncryptedEntryWarning(!options.AllowEncrypted));
                break;
            case RvBankEntryMime.Version:
                LastResult.WithWarning(new RvBankImproperMimeWarning(!options.AllowVersionMimeOnData,
                    typeof(IRvBankDataEntry)));
                break;
            case RvBankEntryMime.Decompressed:
            case RvBankEntryMime.Compressed:
            default:
                break;
        }

        if (EntryName.Length == 0)
        {
            LastResult.WithWarning(new RvBankUnnamedEntryWarning(!options.AllowUnnamedDataEntries, typeof(IRvBankDataEntry)));
        }

        if (DataSize <= 0)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IRvBankDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect DataSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        if (OriginalSize <= 0)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IRvBankDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect OriginalSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        return LastResult;
    }

    public sealed override int CalculateHeaderLength(RvBankOptions options) => base.CalculateHeaderLength(options);

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
