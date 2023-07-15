namespace BisUtils.RVBank.Model.Stubs;

using BisUtils.Core.Family;
using BisUtils.Core.IO;
using FResults;
using Options;

public interface IRVBankVfsEntry : IRVBankElement
{
    IRVBankDirectory? ParentDirectory { get; set; }
    string EntryName { get; set; }
    string Path { get; }
    string AbsolutePath { get; }
}

public abstract class RVBankVfsEntry : RVBankElement, IRVBankVfsEntry
{
    public string EntryName { get; set; } = null!;

    public string Path => ParentDirectory is { } parentDirectory ? parentDirectory.Path + "\\" + EntryName : EntryName;
    public string AbsolutePath => ParentDirectory is { } parentDirectory ? parentDirectory.AbsolutePath + "\\" + EntryName : BankFile.AbsolutePath + "\\" + EntryName;
    public IRVBankDirectory? ParentDirectory { get; set; }

    protected RVBankVfsEntry(IRVBank file, IRVBankDirectory? parent, string name) : base(file)
    {
        ParentDirectory = parent;
        EntryName = name;
    }

    protected RVBankVfsEntry(IRVBank file, IRVBankDirectory? parent, BisBinaryReader reader, RVBankOptions options) :
        base(file, reader, options) => ParentDirectory = parent;



    public override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        LastResult = reader.ReadAsciiZ(out var entryName, options);
        EntryName = entryName;
        return LastResult;
    }


    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        writer.WriteAsciiZ(EntryName, options);
        return LastResult = Result.Ok();
    }
}
