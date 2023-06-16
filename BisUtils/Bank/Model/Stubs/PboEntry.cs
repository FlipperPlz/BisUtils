namespace BisUtils.Bank.Model.Stubs;

using Core.IO;
using FResults;

public interface IPboEntry : IPboVFSEntry
{
    public PboEntryMime EntryMime { get; }
    public long OriginalSize { get; }
    public long Offset { get; }
    public long TimeStamp { get; }
    public long DataSize { get; }
}

public abstract class PboEntry : PboVFSEntry
{
    public PboEntryMime EntryMime { get; } = PboEntryMime.Decompressed;
    public long OriginalSize { get; }
    public long Offset { get; }
    public long TimeStamp { get; }
    public long DataSize { get; }


    public bool IsEmptyMeta() =>
        OriginalSize == 0 &&
        Offset == 0 &&
        TimeStamp == 0 &&
        DataSize == 0;

    protected PboEntry(
        string fileName,
        PboEntryMime mime,
        long originalSize,
        long offset,
        long timeStamp,
        long dataSize
    ) : base(fileName)
    {
        EntryMime = mime;
        OriginalSize = originalSize;
        Offset = offset;
        TimeStamp = timeStamp;
        DataSize = dataSize;
    }

    protected PboEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }


    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        //TODO: Binarize with options

        throw new NotImplementedException();
    }

    public override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        List<Result> results = new() { base.Debinarize(reader, options) };

        //write mime
        //write originalSize
        //write offset
        //write timestamp
        //write size

        return Result.Merge(results);
    }

    public override Result Validate(PboOptions options) => throw new NotImplementedException();
}
