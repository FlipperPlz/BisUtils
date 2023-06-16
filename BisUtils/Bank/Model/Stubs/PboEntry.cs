namespace BisUtils.Bank.Model.Stubs;

using BisUtils.Core.IO;
using FResults;

public interface IPboEntry : IPboVFSEntry
{
    PboEntryMime EntryMime { get; }
    long OriginalSize { get; }
    long Offset { get; }
    long TimeStamp { get; }
    long DataSize { get; }
    bool IsEmptyMeta();
}

public abstract class PboEntry : PboVFSEntry
{
    public Result? LastResult { get; protected set; }
    public PboEntryMime EntryMime { get; set; } = PboEntryMime.Decompressed;
    public long OriginalSize { get; set; }
    public long Offset { get; set;  }
    public long TimeStamp { get; set; }
    public long DataSize { get; set; }

    protected PboEntry(
        IPboFile? file,
        IPboDirectory? parent,
        string fileName,
        PboEntryMime mime,
        long originalSize,
        long offset,
        long timeStamp,
        long dataSize
    ) : base(file, parent, fileName)
    {
        EntryMime = mime;
        OriginalSize = originalSize;
        Offset = offset;
        TimeStamp = timeStamp;
        DataSize = dataSize;
    }


    public bool IsEmptyMeta() =>
        OriginalSize == 0 &&
        Offset == 0 &&
        TimeStamp == 0 &&
        DataSize == 0;

    public bool IsDummyEntry() =>
        IsEmptyMeta() &&
        EntryName == "";

    protected PboEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

}
