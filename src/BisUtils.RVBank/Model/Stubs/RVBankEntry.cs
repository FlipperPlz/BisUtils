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
    uint OriginalSize { get; set; }
    ulong Offset { get; set; }
    ulong TimeStamp { get; set; }
    ulong DataSize { get; set; }

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

    private uint originalSize;
    public virtual uint OriginalSize
    {
        get => originalSize;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            originalSize = value;
        }
    }

    private ulong offset;
    public virtual ulong Offset
    {
        get => offset;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            offset = value;
        }
    }

    private ulong timeStamp;
    public virtual ulong TimeStamp
    {
        get => timeStamp;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            timeStamp = value;
        }
    }

    private ulong dataSize;
    public virtual ulong DataSize
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
        originalSize = (uint)reader.ReadInt32();
        offset = (ulong)reader.ReadInt32();
        timeStamp = (ulong)reader.ReadInt32();
        dataSize = (ulong)reader.ReadInt32();
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
    protected void QuietlySetOriginalSize(int ogSize) => originalSize = unchecked((uint)ogSize);
    protected void QuietlySetTimestamp(long time) => timeStamp = unchecked((ulong)time);

    protected void QuietlySetSize(long size) => dataSize = unchecked((ulong)size);

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
