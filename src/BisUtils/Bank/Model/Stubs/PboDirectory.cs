namespace BisUtils.Bank.Model.Stubs;

using System.Diagnostics;
using BisUtils.Core.Family;
using BisUtils.Core.IO;
using Entry;
using FResults;
using Utils;

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

    IPboDirectory CreateDirectory(string name, IPboFile? node);
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

    public IPboDirectory? GetDirectory(string name)
    {
        var watch = Stopwatch.StartNew();
        Console.WriteLine($"GetDirectory Called On \"{AbsolutePath}\" with {name}");

        var ret = Directories.FirstOrDefault(e => e.EntryName == name);
        watch.Stop();
        Console.WriteLine($"(PboDirectory::GetDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
        return ret;
    }

    public IPboDirectory CreateDirectory(string name, IPboFile? node)
    {

        var watch = Stopwatch.StartNew();

        Console.WriteLine($"CreateDirectory Called On \"{AbsolutePath}\" with {name}");

        var split = name.Split('\\', 2);
        IPboDirectory ret;

        if (split[0].Length == 0)
        {
            watch.Stop();
            Console.WriteLine($"(PboDirectory::CreateDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
            return this;
        }

        if (GetDirectory(split[0]) is { } i)
        {
            if (split[1].Length == 0)
            {
                watch.Stop();
                Console.WriteLine($"(PboDirectory::CreateDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
                return i;
            }
            if(!PboEntries.Contains(i))
            {
                PboEntries.Add(i);
            }
            ret = i.CreateDirectory(split[1], node);
            watch.Stop();
            Console.WriteLine($"(PboDirectory::CreateDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
            return ret;
        }

        var directory = new PboDirectory(node, this, new List<IPboEntry>(), PboPathUtilities.GetFilename(split[0]));
        PboEntries.Add(directory);
        if (split[1].Length == 0)
        {
            watch.Stop();
            Console.WriteLine($"(PboDirectory::CreateDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
            return directory;
        }
        ret = directory.CreateDirectory(split[1], node);

        watch.Stop();

        Console.WriteLine($"(PboDirectory::CreateDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");

        return ret;
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var watch = Stopwatch.StartNew();

        LastResult = Result.Merge
        (
            new List<Result> { Result.ImmutableOk() }.Concat(PboEntries.Select(e => e.Binarize(writer, options)))
        );

        watch.Stop();

        Console.WriteLine($"(PboDirectory::Binarize) Execution Time: {watch.ElapsedMilliseconds} ms");
        return LastResult;
    }

    public override Result Debinarize(BisBinaryReader reader, PboOptions options) =>
        throw new NotSupportedException();

    public override Result Validate(PboOptions options)
    {
        var watch = Stopwatch.StartNew();
        LastResult = Result.Merge(PboEntries.Select(e => e.Validate(options)));
        watch.Stop();
        Console.WriteLine($"(PboDirectory::Validate) Execution Time: {watch.ElapsedMilliseconds} ms");
        return LastResult;
    }

}
