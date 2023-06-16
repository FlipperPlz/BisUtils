namespace BisUtils.Bank.Model.Entry;

using BisUtils.Bank.Alerts.Warnings;
using BisUtils.Core.Binarize.Exceptions;
using BisUtils.Core.IO;
using BisUtils.Bank.Model.Stubs;

using FResults;
using FResults.Reasoning;

public interface IPboDataEntry : IPboEntry
{
    Stream EntryData { get; }
}

public class PboDataEntry : PboEntry, IPboDataEntry
{
    public Stream EntryData { get; set; } = Stream.Null;

    public PboDataEntry
    (
        IPboFile? file,
        IPboDirectory? parent,
        string fileName,
        PboEntryMime mime,
        long originalSize,
        long offset,
        long timeStamp,
        long dataSize
    ) : base(file, parent, fileName, mime, originalSize, offset, timeStamp, dataSize)
    {
    }

    public PboDataEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
        LastResult = Debinarize(reader, options);
        if (LastResult.IsFailed)
        {
            throw new DebinarizeFailedException(LastResult.ToString());
        }
    }

    public sealed override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        LastResult = base.Binarize(writer, options);
        writer.Write((long) EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);
        return LastResult;
    }

    public sealed override Result Validate(PboOptions options)
    {
        LastResult = Result.Ok();

        switch (EntryMime)
        {
            case PboEntryMime.Encrypted:
                LastResult.WithWarning(new PboEncryptedEntryWarning(!options.AllowEncrypted));
                break;
            case PboEntryMime.Version:
                LastResult.WithWarning(new PboImproperMimeWarning(!options.AllowVersionMimeOnData, typeof(IPboDataEntry)));
                break;
            case PboEntryMime.Decompressed:
            case PboEntryMime.Compressed:
            default:
                break;
        }

        if (EntryName.Length == 0)
        {
            LastResult.WithWarning(new PboUnnamedEntryWarning(!options.AllowUnnamedDataEntries, typeof(IPboDataEntry)));
        }

        if (EntryData.Length != DataSize)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IPboDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect Stream/DataSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        if (DataSize <= 0)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IPboDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect DataSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        if (OriginalSize <= 0)
        {
            LastResult.WithWarning(new Warning
            {
                AlertScope = typeof(IPboDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect OriginalSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }
        return LastResult;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        LastResult = base.Debinarize(reader, options);
        EntryMime = (PboEntryMime) reader.ReadInt64();// TODO WARN/ERROR then recover
        OriginalSize = reader.ReadInt64();
        TimeStamp = reader.ReadInt64();
        Offset = reader.ReadInt64();
        DataSize = reader.ReadInt64();

        return LastResult;
    }

    public void SynchronizeMetaWithStream() => OriginalSize = EntryData.Length;

}
