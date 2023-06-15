namespace BisUtils.Bank.Model.Stubs;

using Core.Family;
using Core.IO;
using FResults;

public abstract class PboVFSEntry : PboElement, IFamilyChild
{
    public PboDirectory? ParentDirectory { get; set; }
    public IFamilyParent? Parent => ParentDirectory;

    private string entryName = string.Empty;
    public string EntryName
    {
        get => entryName;
        set => entryName = value;
    }

    public string Path => ParentDirectory?.Path + "\\" + EntryName;
    public string AbsolutePath => ParentDirectory?.AbsolutePath + "\\" + EntryName;

    protected PboVFSEntry(string entryName) : base() => EntryName = entryName;

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
