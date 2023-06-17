namespace BisUtils.Bank.Model.Stubs;

using BisUtils.Core.Family;
using BisUtils.Core.IO;
using Entry;
using FResults;

public interface IPboDirectory : IPboEntry, IFamilyParent
{
    List<IPboEntry> PboEntries { get; }

    int IPboEntry.OriginalSize => PboEntries.Sum(e => e.OriginalSize);

    int IPboEntry.Offset => throw new NotSupportedException();

    int IPboEntry.TimeStamp => throw new NotSupportedException();

    int IPboEntry.DataSize => PboEntries.Sum(e => e.DataSize);

    PboEntryMime IPboEntry.EntryMime => throw new NotSupportedException();

    IEnumerable<IPboDataEntry> FileEntries { get;  }

    IEnumerable<IPboDirectory> Directories { get; }

    IEnumerable<IPboVFSEntry> VfsEntries => PboEntries.OfType<IPboVFSEntry>().ToList();

    IEnumerable<IFamilyMember> IFamilyParent.Children => VfsEntries;

    IPboDirectory? GetDirectory(string name);

    IPboDirectory CreateDirectory(string name);
}

public class PboDirectory : PboVFSEntry, IPboDirectory
{
    public List<IPboEntry> PboEntries { get; set; } = new();
    public IEnumerable<IPboDataEntry> FileEntries => PboEntries.OfType<IPboDataEntry>();
    public IEnumerable<IPboDirectory> Directories => PboEntries.OfType<IPboDirectory>();

    public PboDirectory(
        IPboFile? file,
        IPboDirectory? parent,
        List<IPboEntry> entries,
        string directoryName
    ) : base(file, parent, directoryName) => PboEntries = entries;

    protected PboDirectory(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }


    public IPboDirectory? GetDirectory(string name) =>
        Directories.FirstOrDefault(e => e.EntryName == name);

    public IPboDirectory CreateDirectory(string name)
    {
        var split = name.Split('\\', 2);

        if (GetDirectory(split[0]) is { } i)
        {
            return i.CreateDirectory(split[1]);
        }

        var directory = new PboDirectory(PboFile, this, new List<IPboEntry>(), split[0]);
        PboEntries.Add(directory);

        return directory.CreateDirectory(split[1]);
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options) => LastResult = Result.Merge
    (
        new List<Result>
        {
             Result.ImmutableOk()
        }.Concat(PboEntries.Select(e => e.Binarize(writer, options)))
    );

    public override Result Debinarize(BisBinaryReader reader, PboOptions options) =>
        throw new NotSupportedException();

    public override Result Validate(PboOptions options) =>
        LastResult = Result.Merge(PboEntries.Select(e => e.Validate(options)));
}
