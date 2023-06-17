namespace BisUtils.Bank.Model.Stubs;

using System.Diagnostics;
using BisUtils.Core.Family;
using BisUtils.Core.IO;
using FResults;
using Options;

public interface IPboVFSEntry : IPboElement, IFamilyChild
{
    IPboDirectory? ParentDirectory { get; }

    string EntryName { get; }

    string Path { get; }

    string AbsolutePath { get; }

    IFamilyParent? IFamilyChild.Parent => ParentDirectory;
}

public abstract class PboVFSEntry : PboElement, IPboVFSEntry
{
    public string Path => ParentDirectory?.Path + "\\" + EntryName;
    public string AbsolutePath => ParentDirectory?.AbsolutePath + "\\" + EntryName;
    public IPboDirectory? ParentDirectory { get; set; }
    private string entryName = string.Empty;
    public string EntryName { get => entryName; set => entryName = value; }

    protected PboVFSEntry(IPboFile? file, IPboDirectory? parent, string name) : base(file)
    {
        ParentDirectory = parent;
        EntryName = name;
    }

    protected PboVFSEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif

        LastResult = reader.ReadAsciiZ(out entryName, options);
#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboVFSEntry::Debinarize) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif
        return LastResult;
    }


    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
#if DEBUG
        var watch = Stopwatch.StartNew();
#endif

        writer.WriteAsciiZ(entryName, options);
        LastResult = Result.ImmutableOk();

#if DEBUG
        watch.Stop();
        Console.WriteLine($"(PboVFSEntry::Binarize) Execution Time: {watch.ElapsedMilliseconds} ms");
#endif

        return LastResult;
    }
}
