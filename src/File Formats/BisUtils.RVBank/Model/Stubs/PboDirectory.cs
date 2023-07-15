namespace BisUtils.RVBank.Model.Stubs;

using BisUtils.Core.Family;
using BisUtils.Core.IO;
using BisUtils.RVBank.Enumerations;
using BisUtils.RVBank.Model.Entry;
using FResults;
using Options;
using Utils;

public interface IPboDirectory : IPboEntry, IFamilyParent
{
    List<IPboEntry> PboEntries { get; }

    IEnumerable<IPboDataEntry> FileEntries { get; }

    IEnumerable<IPboDirectory> Directories { get; }

    IEnumerable<IPboVFSEntry> VfsEntries => PboEntries.OfType<IPboVFSEntry>().ToList();

    IEnumerable<IFamilyMember> IFamilyParent.Children => VfsEntries;

    int IPboEntry.OriginalSize => PboEntries.Sum(e => e.OriginalSize);

    int IPboEntry.Offset => throw new NotSupportedException();

    int IPboEntry.TimeStamp => throw new NotSupportedException();

    int IPboEntry.DataSize => PboEntries.Sum(e => e.DataSize);

    PboEntryMime IPboEntry.EntryMime => throw new NotSupportedException();

    IPboDirectory? GetDirectory(string name);

    IPboDirectory CreateDirectory(string name, IPboFile? node);
}

public class PboDirectory : PboVFSEntry, IPboDirectory
{
    public PboDirectory(
        IPboFile? file,
        IPboDirectory? parent,
        List<IPboEntry> entries,
        string directoryName
    ) : base(file, parent, directoryName) => PboEntries = entries;

    protected PboDirectory(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public List<IPboEntry> PboEntries { get; set; } = new();
    public IEnumerable<IPboDataEntry> FileEntries => PboEntries.OfType<IPboDataEntry>();
    public IEnumerable<IPboDirectory> Directories => PboEntries.OfType<IPboDirectory>();

    public IPboDirectory? GetDirectory(string name) =>
        Directories.FirstOrDefault(e => e.EntryName == name);

    public IPboDirectory CreateDirectory(string name, IPboFile? node)
    {
        var split = name.Split('\\', 2);
        IPboDirectory ret;

        if (split[0].Length == 0)
        {
            return this;
        }

        if (GetDirectory(split[0]) is { } i)
        {
            if (split[1].Length == 0)
            {
                return i;
            }

            if (!PboEntries.Contains(i))
            {
                PboEntries.Add(i);
            }

            ret = i.CreateDirectory(split[1], node);

            return ret;
        }

        var directory = new PboDirectory(node, this, new List<IPboEntry>(), PboPathUtilities.GetFilename(split[0]));
        PboEntries.Add(directory);
        if (split[1].Length == 0)
        {
            return directory;
        }

        ret = directory.CreateDirectory(split[1], node);

        return ret;
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        LastResult = Result.Merge
        (
            new List<Result> { Result.ImmutableOk() }.Concat(PboEntries.Select(e => e.Binarize(writer, options)))
        );

        return LastResult;
    }

    public override Result Debinarize(BisBinaryReader reader, PboOptions options) =>
        throw new NotSupportedException();

    public override Result Validate(PboOptions options)
    {
        LastResult = Result.Merge(PboEntries.Select(e => e.Validate(options)));
        return LastResult;
    }
}
