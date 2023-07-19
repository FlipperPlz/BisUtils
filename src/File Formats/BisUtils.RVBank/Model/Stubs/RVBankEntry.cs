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
    private RVBankEntryMime entryMime = RVBankEntryMime.Decompressed;
    public RVBankEntryMime EntryMime
    {
        get => entryMime;
        set
        {
            OnChangesMade(EventArgs.Empty);
            entryMime = value;
        }
    }

    private int originalSize;
    public int OriginalSize
    {
        get => originalSize;
        set
        {
            OnChangesMade(EventArgs.Empty);
            originalSize = value;
        }
    }

    private int offset;
    public int Offset
    {
        get => offset;
        set
        {
            OnChangesMade(EventArgs.Empty);
            offset = value;
        }
    }

    private int timeStamp;
    public int TimeStamp
    {
        get => timeStamp;
        set
        {
            OnChangesMade(EventArgs.Empty);
            timeStamp = value;
        }
    }

    private int dataSize;
    public int DataSize
    {
        get => dataSize;
        set
        {
            OnChangesMade(EventArgs.Empty);
            dataSize = value;
        }
    }

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
