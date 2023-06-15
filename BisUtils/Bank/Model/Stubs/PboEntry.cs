namespace BisUtils.Bank.Model.Stubs;

using Core.IO;
using FResults;

public abstract class PboEntry : PboVFSEntry
{
    public PboEntryMime EntryMime { get; set; } = PboEntryMime.Decompressed;
    public long OriginalSize { get; set; }
    public long Offset { get; set; }
    public long TimeStamp { get; set; }
    public long DataSize { get; set; }

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

    public bool IsEmptyMeta() =>
        OriginalSize == 0 &&
        Offset == 0 &&
        TimeStamp == 0 &&
        DataSize == 0;

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
}
