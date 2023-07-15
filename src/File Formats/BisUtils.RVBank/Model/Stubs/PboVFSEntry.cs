namespace BisUtils.RVBank.Model.Stubs;

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
    private string entryName = string.Empty;

    protected PboVFSEntry(IPboFile? file, IPboDirectory? parent, string name) : base(file)
    {
        ParentDirectory = parent;
        EntryName = name;
    }

    protected PboVFSEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public string Path => ParentDirectory?.Path + "\\" + EntryName;
    public string AbsolutePath => ParentDirectory?.AbsolutePath + "\\" + EntryName;
    public IPboDirectory? ParentDirectory { get; set; }
    public string EntryName { get => entryName; set => entryName = value; }

    public override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        LastResult = reader.ReadAsciiZ(out entryName, options);

        return LastResult;
    }


    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        writer.WriteAsciiZ(entryName, options);
        LastResult = Result.ImmutableOk();

        return LastResult;
    }
}
