namespace BisUtils.RVBank.Model.Stubs;

using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IRVBankVfsEntry : IRVBankElement
{
    IRVBankDirectory ParentDirectory { get; }
    string EntryName { get; set; }
    string Path { get; }
    string AbsolutePath { get; }
}

public abstract class RVBankVfsEntry : RVBankElement, IRVBankVfsEntry
{
    private string entryName = null!;
    public string EntryName
    {
        get => entryName;
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            entryName = value;
        }
    }

    public string Path => (ParentDirectory is { } parentDirectory ? parentDirectory.Path + "\\" + EntryName : EntryName).TrimStart('\\');
    public string AbsolutePath => (ParentDirectory is { } parentDirectory ? parentDirectory.AbsolutePath + "\\" + EntryName : BankFile.AbsolutePath + "\\" + EntryName).TrimStart('\\');


    public IRVBankDirectory ParentDirectory { get; protected set; }

    protected RVBankVfsEntry(string name, IRVBank file, IRVBankDirectory parent, ILogger? logger) : base(file, logger)
    {
        ParentDirectory = parent;
        EntryName = name;
    }

    protected RVBankVfsEntry(BisBinaryReader reader, RVBankOptions options, IRVBank file, IRVBankDirectory parent, ILogger? logger) :
        base(reader, options, file, logger) => ParentDirectory = parent;

    public override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        LastResult = reader.ReadAsciiZ(out entryName, options);
        return LastResult;
    }


    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        writer.WriteAsciiZ(EntryName, options);
        return LastResult = Result.Ok();
    }
}
