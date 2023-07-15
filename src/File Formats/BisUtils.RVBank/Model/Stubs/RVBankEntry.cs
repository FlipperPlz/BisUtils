namespace BisUtils.RVBank.Model.Stubs;

using Core.IO;
using Enumerations;
using Options;

public interface IRVBankEntry : IRVBankVfsEntry
{
    RVBankEntryMime EntryMime { get; }
    int OriginalSize { get; }
    int Offset { get; }
    int TimeStamp { get; }
    int DataSize { get; }

    bool IsEmptyMeta() =>
        OriginalSize == 0 && Offset == 0 && TimeStamp == 0 && DataSize == 0;

    bool IsDummyEntry() => IsEmptyMeta() && EntryName == "";
}

public abstract class RVBankEntry : RVBankVfsEntry
{
    public RVBankEntryMime EntryMime { get; set; } = RVBankEntryMime.Decompressed;
    public int OriginalSize { get; set; }
    public int Offset { get; set; }
    public int TimeStamp { get; set; }
    public int DataSize { get; set; }

    protected RVBankEntry(
        IRVBank file,
        IRVBankDirectory parent,
        string fileName,
        RVBankEntryMime mime,
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

    protected RVBankEntry(IRVBank file, IRVBankDirectory parent, BisBinaryReader reader, RVBankOptions options) : base(file, parent, reader, options)
    {
    }

    public bool IsEmptyMeta() =>
        OriginalSize == 0 && Offset == 0 && TimeStamp == 0 && DataSize == 0;

    public bool IsDummyEntry() => IsEmptyMeta() && EntryName == "";
}
