namespace BisUtils.RVBank.Model.Stubs;

using BisUtils.Core.IO;
using BisUtils.RVBank.Enumerations;
using Options;

public interface IPboEntry : IPboVFSEntry
{
    PboEntryMime EntryMime { get; }
    int OriginalSize { get; }
    int Offset { get; }
    int TimeStamp { get; }
    int DataSize { get; }

    bool IsEmptyMeta() =>
        OriginalSize == 0 && Offset == 0 && TimeStamp == 0 && DataSize == 0;

    bool IsDummyEntry() => IsEmptyMeta() && EntryName == "";
}

public abstract class PboEntry : PboVFSEntry
{
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

    public PboEntryMime EntryMime { get; set; } = PboEntryMime.Decompressed;
    public int OriginalSize { get; set; }
    public int Offset { get; set; }
    public int TimeStamp { get; set; }
    public int DataSize { get; set; }

    public bool IsEmptyMeta() =>
        OriginalSize == 0 && Offset == 0 && TimeStamp == 0 && DataSize == 0;

    public bool IsDummyEntry() => IsEmptyMeta() && EntryName == "";
}
