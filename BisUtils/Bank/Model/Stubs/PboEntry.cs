namespace BisUtils.Bank.Model.Stubs;

using BisUtils.Core.IO;
using FResults;

public interface IPboEntry : IPboVFSEntry
{
    Result? LastResult { get; }
    PboEntryMime EntryMime { get; }
    int OriginalSize { get; }
    int Offset { get; }
    int TimeStamp { get; }
    int DataSize { get; }

    bool IsEmptyMeta() =>
        OriginalSize == 0 &&
        Offset == 0 &&
        TimeStamp == 0 &&
        DataSize == 0;

    bool IsDummyEntry() =>
        IsEmptyMeta() &&
        EntryName == "";
}

public abstract class PboEntry : PboVFSEntry
{
    public Result? LastResult { get; protected set; }
    public PboEntryMime EntryMime { get; set; } = PboEntryMime.Decompressed;
    public int OriginalSize { get; set; }
    public int Offset { get; set;  }
    public int TimeStamp { get; set; }
    public int DataSize { get; set; }

    protected PboEntry(
        IPboFile? file,
        IPboDirectory? parent,
        string fileName,
        PboEntryMime mime,
        int originalSize,
        int offset,
        int timeStamp,
        int dataSize
    ) : base(file, parent, fileName)
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

    public bool IsDummyEntry() =>
        IsEmptyMeta() &&
        EntryName == "";
}
