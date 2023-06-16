namespace BisUtils.Bank.Model.Entry;

using Alerts.Warnings;
using Core.Binarize.Exceptions;
using Core.IO;
using FResults;
using Stubs;

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

    public new Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var result = base.Binarize(writer, options);
        writer.Write((long) EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);
        return result;
    }

    public new Result Validate(PboOptions options)
    {
        var results = new List<Result>() { base.Validate(options)};

        switch (EntryMime)
        {
            case PboEntryMime.Encrypted:
                results.Add(new Result().WithWarning(new PboEncryptedEntryWarning(!options.AllowEncrypted)));
                break;
            case PboEntryMime.Version:
                results.Add(new Result().WithWarning(new PboImproperMimeWarning(!options.AllowVersionMimeOnData, typeof(IPboDataEntry))));
                break;
            case PboEntryMime.Decompressed:
            case PboEntryMime.Compressed:
            default:
                break;
        }

        if (EntryName.Length == 0)
        {
            results.Add(new Result().WithWarning(new PboUnnamedEntryWarning(!options.AllowUnnamedDataEntries, typeof(IPboDataEntry))));
        }

        //TODO: Check Lengths/Sizes


        return Result.Merge(results);

    }

    public new Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        var result = base.Debinarize(reader, options);
        EntryMime = (PboEntryMime) reader.ReadInt64();// TODO WARN/ERROR then recover
        OriginalSize = reader.ReadInt64();
        TimeStamp = reader.ReadInt64();
        Offset = reader.ReadInt64();
        DataSize = reader.ReadInt64();

        return result;
    }
}
