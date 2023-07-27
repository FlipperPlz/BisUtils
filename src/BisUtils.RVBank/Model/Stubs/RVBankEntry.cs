namespace BisUtils.RVBank.Model.Stubs;

using Core.IO;
using Core.Parsing;
using Entry;
using Enumerations;
using Extensions;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IRVBankEntry : IRVBankElement
{
    IRVBankDirectory ParentDirectory { get; set; }
    string Path { get; }
    string AbsolutePath { get; }

    string EntryName { get; set; }
    RVBankEntryMime EntryMime { get; set; }
    int OriginalSize { get; set; }
    int Offset { get; set; }
    int TimeStamp { get; set; }
    int DataSize { get; set; }

    void Move(IRVBankDirectory directory);
    void Delete();
    IEnumerable<IRVBankEntry> MoveAndReplace(IRVBankDirectory directory);

    int CalculateHeaderLength(RVBankOptions options);
}

public abstract class RVBankEntry : RVBankElement, IRVBankEntry
{
    public IRVBankDirectory ParentDirectory { get; set; }
    public virtual string Path => $"{ParentDirectory.Path}\\{EntryName}";
    public virtual string AbsolutePath => $"{ParentDirectory.AbsolutePath}\\{EntryName}";

    private string entryName = "";
    public virtual string EntryName
    {
        get => RVPathUtilities.NormalizePboPath(entryName);
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            entryName = value;
        }
    }

    private RVBankEntryMime entryMime = RVBankEntryMime.Decompressed;
    public virtual RVBankEntryMime EntryMime
    {
        get => entryMime;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            entryMime = value;
        }
    }

    private int originalSize;
    public virtual int OriginalSize
    {
        get => originalSize;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            originalSize = value;
        }
    }

    private int offset;
    public virtual int Offset
    {
        get => offset;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            offset = value;
        }
    }

    private int timeStamp;
    public virtual int TimeStamp
    {
        get => timeStamp;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            timeStamp = value;
        }
    }

    private int dataSize;
    public virtual int DataSize
    {
        get => dataSize;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            dataSize = value;
        }
    }
    //TODO: Constructor with path maybe and no parent arg

    protected RVBankEntry(
        IRVBank file,
        IRVBankDirectory parent,
        ILogger? logger
    ) : base(file, logger) =>
        ParentDirectory = parent;

    protected RVBankEntry(BisBinaryReader reader, RVBankOptions options, IRVBank file, IRVBankDirectory parent, ILogger? logger) : base(reader, options, file, logger) =>
        ParentDirectory = parent;

    public override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        reader.ReadAsciiZ(out entryName, options);
        entryMime = (RVBankEntryMime)reader.ReadInt32();
        originalSize = reader.ReadInt32();
        offset = reader.ReadInt32();
        timeStamp = reader.ReadInt32();
        dataSize = reader.ReadInt32();
        return LastResult = Result.Ok();
    }

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        writer.WriteAsciiZ(Path, options);
        writer.Write((int) EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);
        return LastResult = Result.Ok();
    }

    protected void QuietlySetName(string name) => entryName = name;
    protected void QuietlySetMime(RVBankEntryMime mime) => entryMime = mime;
    protected void QuietlySetOriginalSize(int ogSize) => originalSize = ogSize;
    protected void QuietlySetTimestamp(int time) => timeStamp = time;

    protected void QuietlySetSize(int size) => dataSize = size;

    public virtual void Move(IRVBankDirectory directory)
    {
        if (directory.BankFile != BankFile)
        {
            throw new IOException("Cannot move this entry to a directory outside of the current pbo.");
        }
        ParentDirectory.RemoveEntry(this);
        ParentDirectory = directory;
        ParentDirectory.PboEntries.Add(this);
        OnChangesMade(this, EventArgs.Empty);
    }


    public virtual void Delete()
    {
        ParentDirectory.RemoveEntry(this);
        OnChangesMade(this, EventArgs.Empty);
    }

    public virtual IEnumerable<IRVBankEntry> MoveAndReplace(IRVBankDirectory directory)
    {
        Move(directory);
        foreach (var duplicate in this.RetrieveDuplicateEntries())
        {
            yield return duplicate;
        }
    }

    public virtual int CalculateHeaderLength(RVBankOptions options) => 21 + options.Charset.GetByteCount(Path);

}
