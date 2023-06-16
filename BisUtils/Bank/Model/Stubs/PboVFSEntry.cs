namespace BisUtils.Bank.Model.Stubs;

using Core.Family;
using Core.IO;
using FResults;

public interface IPboVFSEntry : IPboElement, IFamilyChild
{
    IPboDirectory? ParentDirectory { get; }

    string EntryName { get; }

    string Path => ParentDirectory?.Path + "\\" + EntryName;

    string AbsolutePath => ParentDirectory?.AbsolutePath + "\\" + EntryName;

    IFamilyParent? IFamilyChild.Parent => ParentDirectory;
}

public abstract class PboVFSEntry : PboElement, IPboVFSEntry
{
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

    public override Result Debinarize(BisBinaryReader reader, PboOptions options) =>
        reader.ReadAsciiZ(out entryName, options);

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        writer.WriteAsciiZ(entryName, options);
        return Result.ImmutableOk();
    }
}
