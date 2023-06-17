namespace BisUtils.Bank.Model.Stubs;

using System.Diagnostics;
using Core.Family;
using Core.IO;
using Entry;
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

    public IPboDirectory? GetDirectory(string name)
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
        Console.WriteLine($"GetDirectory Called On \"{AbsolutePath}\" with {name}");

#endif

        var ret = Directories.FirstOrDefault(e => e.EntryName == name);
#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboDirectory::GetDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif
        return ret;
    }

    public IPboDirectory CreateDirectory(string name, IPboFile? node)
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
        Console.WriteLine($"CreateDirectory Called On \"{AbsolutePath}\" with {name}");
#endif

        var split = name.Split('\\', 2);
        IPboDirectory ret;

        if (split[0].Length == 0)
        {
#if DEBUG
            watch.Stop();
            Console.WriteLine($"(PboDirectory::CreateDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

            return this;
        }

        if (GetDirectory(split[0]) is { } i)
        {
            if (split[1].Length == 0)
            {
#if DEBUG
                watch.Stop();
                Console.WriteLine($"(PboDirectory::CreateDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

                return i;
            }

            if (!PboEntries.Contains(i))
            {
                PboEntries.Add(i);
            }

            ret = i.CreateDirectory(split[1], node);
#if DEBUG
            watch.Stop();
            Console.WriteLine($"(PboDirectory::CreateDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif
            return ret;
        }

        var directory = new PboDirectory(node, this, new List<IPboEntry>(), PboPathUtilities.GetFilename(split[0]));
        PboEntries.Add(directory);
        if (split[1].Length == 0)
        {
#if DEBUG
            watch.Stop();
            Console.WriteLine($"(PboDirectory::CreateDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif
            return directory;
        }

        ret = directory.CreateDirectory(split[1], node);

#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboDirectory::CreateDirectory) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

        return ret;
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif


        LastResult = Result.Merge
        (
            new List<Result> { Result.ImmutableOk() }.Concat(PboEntries.Select(e => e.Binarize(writer, options)))
        );

#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboDirectory::Binarize) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

        return LastResult;
    }

    public override Result Debinarize(BisBinaryReader reader, PboOptions options) =>
        throw new NotSupportedException();

    public override Result Validate(PboOptions options)
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif

        LastResult = Result.Merge(PboEntries.Select(e => e.Validate(options)));
#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboDirectory::Validate) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

        return LastResult;
    }
}
