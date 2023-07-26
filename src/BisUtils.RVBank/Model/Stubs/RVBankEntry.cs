namespace BisUtils.RVBank.Model.Stubs;

using Core.IO;
using Enumerations;
using Extensions;
using Microsoft.Extensions.Logging;
using Options;

public interface IRVBankEntry : IRVBankVfsEntry
{
    RVBankEntryMime EntryMime { get; }
    uint OriginalSize { get; }
    uint Offset { get; }
    uint TimeStamp { get; set; }
    uint DataSize { get;  }

    bool IsEmptyMeta() =>
        OriginalSize == 0 && Offset == 0 && TimeStamp == 0 && DataSize == 0;
    bool IsDummyEntry() => IsEmptyMeta() && EntryName == "";

    uint CalculateLength(RVBankOptions options);
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


    private uint originalSize;
    public uint OriginalSize
    {
        get => originalSize;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            originalSize = value;
        }
    }

    private uint offset;
    public uint Offset
    {
        get => offset;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            offset = value;
        }
    }

    private uint timeStamp;
    public uint TimeStamp
    {
        get => timeStamp;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            timeStamp = value;
        }
    }

    private uint dataSize;
    public uint DataSize
    {
        get => dataSize;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            dataSize = value;
        }
    }

    protected RVBankEntry(
        string fileName,
        RVBankEntryMime mime,
        uint originalSize,
        uint offset,
        uint timeStamp,
        uint dataSize,
        IRVBank file,
        IRVBankDirectory parent,
        ILogger? logger
    ) : base(fileName, file, parent, logger)
    {

        EntryMime = mime;
        OriginalSize = originalSize;
        Offset = offset;
        TimeStamp = timeStamp;
        DataSize = dataSize;
    }


    protected RVBankEntry(BisBinaryReader reader, RVBankOptions options, IRVBank file, IRVBankDirectory parent,
        ILogger? logger) : base(reader, options, file, parent, logger)
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
        destination.PboEntries.Add(this);
        OnChangesMade(this, EventArgs.Empty);
    }

    public bool IsEmptyMeta() =>
        OriginalSize == 0 && Offset == 0 && TimeStamp == 0 && DataSize == 0;

    public bool IsDummyEntry() => IsEmptyMeta() && EntryName == "";
    public abstract uint CalculateLength(RVBankOptions options);
}
