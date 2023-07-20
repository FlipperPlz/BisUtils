namespace BisUtils.RVBank.Model.Stubs;

using Core.IO;
using Enumerations;
using Extensions;
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
    void Move(IRVBankDirectory destination);

}

public abstract class RVBankEntry : RVBankVfsEntry, IRVBankEntry
{
    private RVBankEntryMime entryMime = RVBankEntryMime.Decompressed;
    public RVBankEntryMime EntryMime
    {
        get => entryMime;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            entryMime = value;
        }
    }


    private int originalSize;
    public int OriginalSize
    {
        get => originalSize;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            originalSize = value;
        }
    }

    private int offset;
    public int Offset
    {
        get => offset;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            offset = value;
        }
    }

    private int timeStamp;
    public int TimeStamp
    {
        get => timeStamp;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            timeStamp = value;
        }
    }

    private int dataSize;
    public int DataSize
    {
        get => dataSize;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
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

    public void Move(IRVBankDirectory destination)
    {
        if (destination.BankFile != BankFile)
        {
            throw new IOException("Cannot move this entry to a directory outside of the current pbo.");
        }

        ParentDirectory.RemoveEntry(this);
        ParentDirectory = destination;
        OnChangesMade(this, EventArgs.Empty);
    }

    public bool IsEmptyMeta() =>
        OriginalSize == 0 && Offset == 0 && TimeStamp == 0 && DataSize == 0;

    public bool IsDummyEntry() => IsEmptyMeta() && EntryName == "";
}
