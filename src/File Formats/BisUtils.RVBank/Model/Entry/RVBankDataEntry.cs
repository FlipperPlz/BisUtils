namespace BisUtils.RVBank.Model.Entry;

using Core.Binarize.Exceptions;
using Core.IO;
using Alerts.Warnings;
using Core.Parsing;
using Enumerations;
using Extensions;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Options;
using Stubs;

public interface IRVBankDataEntry : IRVBankEntry
{
    Stream EntryData { get; }
    RVBankDataType PackingMethod { get; set; }
    void ExpandDirectoryStructure();
}

public class RVBankDataEntry : RVBankEntry, IRVBankDataEntry
{
    public RVBankDataType PackingMethod { get; set; }


    public RVBankDataEntry
    (
        IRVBank file,
        IRVBankDirectory parent,
        string fileName,
        RVBankEntryMime mime,
        int originalSize,
        int offset,
        int timeStamp,
        int dataSize
    ) : base(file, parent, fileName, mime, originalSize, offset, timeStamp, dataSize)
    {
    }

    public RVBankDataEntry(IRVBank file, IRVBankDirectory parent, BisBinaryReader reader, RVBankOptions options) : base(file, parent, reader, options)
    {
        Debinarize(reader, options);
        if (LastResult!.IsFailed)
        {
            throw new DebinarizeFailedException(LastResult.ToString());
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


    public void SynchronizeMetaWithStream() => OriginalSize = (int)EntryData.Length;

    public void ExpandDirectoryStructure()
    {
        ArgumentNullException.ThrowIfNull(BankFile, "When expanding a Pbo Entry, The node must be established");

        var normalizePath = EntryName = RVPathUtilities.NormalizePboPath(EntryName);

        if (!EntryName.Contains('\\'))
        {
            return;
        }
        EntryName = RVPathUtilities.GetFilename(EntryName);

        ParentDirectory = BankFile.CreateDirectory(RVPathUtilities.GetParent(normalizePath), BankFile);
        Move(ParentDirectory);
    }

    public sealed override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        LastResult = base.Binarize(writer, options);
        writer.Write((long)EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);
        IdentifyPackingMethod();
        return LastResult;
    }

    private void IdentifyPackingMethod()
    {
        if (EntryMime is RVBankEntryMime.Encrypted)
        {
            PackingMethod = RVBankDataType.Encrypted;
            return;
        }

        if (OriginalSize <= 0)
        {
            PackingMethod = RVBankDataType.Original;
            return;
        }

        if (DataSize != OriginalSize)
        {
            PackingMethod = RVBankDataType.Compressed;
            return;
        }

        PackingMethod = RVBankDataType.Original;
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
        OriginalSize = reader.ReadInt32();
        TimeStamp = reader.ReadInt32();
        Offset = reader.ReadInt32();
        DataSize = reader.ReadInt32();

        return LastResult;
    }

}
