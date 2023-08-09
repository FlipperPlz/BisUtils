namespace BisUtils.RvBank.Model.Stubs;

using Core.IO;
using Core.Parsing;
using Entry;
using Enumerations;
using Extensions;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IRvBankEntry : IRvBankElement
{
    IRvBankDirectory ParentDirectory { get; set; }
    string Path { get; }
    string AbsolutePath { get; }

    string EntryName { get; set; }
    RvBankEntryMime EntryMime { get; set; }
    uint OriginalSize { get; set; }
    ulong Offset { get; set; }
    ulong TimeStamp { get; set; }
    ulong DataSize { get; set; }

    void Move(IRvBankDirectory directory);
    void Delete();
    IEnumerable<IRvBankEntry> MoveAndReplace(IRvBankDirectory directory);

    int CalculateHeaderLength(RvBankOptions options);
}

public abstract class RvBankEntry : RvBankElement, IRvBankEntry
{
    public IRvBankDirectory ParentDirectory { get; set; }
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

    private RvBankEntryMime entryMime = RvBankEntryMime.Decompressed;
    public virtual RvBankEntryMime EntryMime
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

    protected RvBankEntry(
        IRvBank file,
        IRvBankDirectory parent,
        ILogger? logger
    ) : base(file, logger) =>
        ParentDirectory = parent;

    protected RvBankEntry(BisBinaryReader reader, RvBankOptions options, IRvBank file, IRvBankDirectory parent, ILogger? logger) : base(reader, options, file, logger) =>
        ParentDirectory = parent;

    public override Result Debinarize(BisBinaryReader reader, RvBankOptions options)
    {
        reader.ReadAsciiZ(out entryName, options);
        entryMime = (RvBankEntryMime)reader.ReadInt32();
        originalSize = reader.ReadUInt32();
        offset = reader.ReadUInt32();
        timeStamp = reader.ReadUInt32();
        dataSize = reader.ReadUInt32();
        return LastResult = Result.Ok();
    }

    public override Result Binarize(BisBinaryWriter writer, RvBankOptions options)
    {
        writer.WriteAsciiZ(Path, options);
        writer.Write((int) EntryMime);
        writer.Write(OriginalSize);
        writer.Write(unchecked((uint)Offset));
        writer.Write(unchecked((uint)TimeStamp));
        writer.Write(unchecked((uint)DataSize));
        return LastResult = Result.Ok();
    }

    protected void QuietlySetName(string name) => entryName = name;
    protected void QuietlySetMime(RvBankEntryMime mime) => entryMime = mime;
    protected void QuietlySetOriginalSize(int ogSize) => originalSize = unchecked((uint)ogSize);
    protected void QuietlySetTimestamp(long time) => timeStamp = unchecked((ulong)time);

    protected void QuietlySetSize(long size) => dataSize = unchecked((ulong)size);

    public virtual void Move(IRvBankDirectory directory)
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

    public virtual IEnumerable<IRvBankEntry> MoveAndReplace(IRvBankDirectory directory)
    {
        Move(directory);
        foreach (var duplicate in this.RetrieveDuplicateEntries())
        {
            yield return duplicate;
        }
    }

    public virtual int CalculateHeaderLength(RvBankOptions options) => 21 + options.Charset.GetByteCount(Path);

}
