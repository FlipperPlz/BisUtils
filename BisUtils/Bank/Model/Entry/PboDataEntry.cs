namespace BisUtils.Bank.Model.Entry;

using Core.Family;
using Stubs;

public interface IPboDataEntry : IPboEntry
{
    Stream EntryData { get; }

    new IFamilyNode? Node => PboFile;

    IFamilyParent? IFamilyChild.Parent => ParentDirectory;
}

public class PboDataEntry : IPboDataEntry
{
    public PboFile? PboFile { get; set; }

    public IPboDirectory? ParentDirectory { get; set; }

    public string EntryName { get; set; } = "";

    public PboEntryMime EntryMime { get; set; } = PboEntryMime.Decompressed;

    public long OriginalSize { get; set; }

    public long Offset { get; set; }

    public long TimeStamp { get; set; }

    public long DataSize { get; set; }

    public Stream EntryData { get; set; } = Stream.Null;
}
