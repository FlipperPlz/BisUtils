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
        var result = Debinarize(reader, options);
        if (result.IsFailed)
        {
            throw new DebinarizeFailedException(result.ToString());
        }
    }

    public sealed override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var result = base.Binarize(writer, options);
        writer.Write((long) EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);
        return result;
    }

    public sealed override Result Validate(PboOptions options)
    {
        var result = base.Validate(options);

        switch (EntryMime)
        {
            case PboEntryMime.Encrypted:
                result.WithWarning(new PboEncryptedEntryWarning(!options.AllowEncrypted));
                break;
            case PboEntryMime.Version:
                result.WithWarning(new PboImproperMimeWarning(!options.AllowVersionMimeOnData, typeof(IPboDataEntry)));
                break;
            case PboEntryMime.Decompressed:
            case PboEntryMime.Compressed:
            default:
                break;
        }

        if (EntryName.Length == 0)
        {
            result.WithWarning(new PboUnnamedEntryWarning(!options.AllowUnnamedDataEntries, typeof(IPboDataEntry)));
        }

        if (EntryData.Length != DataSize)
        {
            result.WithWarning(new Warning
            {
                AlertScope = typeof(IPboDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect Stream/DataSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        if (DataSize <= 0)
        {
            result.WithWarning(new Warning
            {
                AlertScope = typeof(IPboDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect DataSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }

        if (OriginalSize <= 0)
        {
            result.WithWarning(new Warning
            {
                AlertScope = typeof(IPboDataEntry),
                AlertName = "EntryReadError",
                Message = "Incorrect OriginalSize Value.",
                IsError = !options.IgnoreInvalidStreamSize
            });
        }
        return result;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        var result = base.Debinarize(reader, options);
        EntryMime = (PboEntryMime) reader.ReadInt64();// TODO WARN/ERROR then recover
        OriginalSize = reader.ReadInt64();
        TimeStamp = reader.ReadInt64();
        Offset = reader.ReadInt64();
        DataSize = reader.ReadInt64();

        return result;
    }

    public void SynchronizeMetaWithStream() => OriginalSize = EntryData.Length;

}
