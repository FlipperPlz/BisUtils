namespace BisUtils.EnfPack.Models;

using System.Text;
using Core.Binarize.Synchronization;
using Core.IO;
using Core.Parsing;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IEsPackEntry : IBisSynchronizableElement<EsPackOptions>, IEsPackElement
{
    public IEsPackDirectory ParentDirectory { get; }
    public string EntryName { get; set; }
    public string Path { get; }
    public string AbsolutePath { get; }
}

public abstract class EsPackEntry : EsPackElement, IEsPackEntry
{
    public IEsPackDirectory ParentDirectory { get; private set; }
    public virtual string Path => $"{ParentDirectory.Path}\\{EntryName}";
    public virtual string AbsolutePath => $"{ParentDirectory.AbsolutePath}\\{EntryName}";
    private string entryName = "";

    public virtual string EntryName
    {
        get => RVPathUtilities.NormalizePboPath(entryName);
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            entryName = value;
        }
    }

    protected EsPackEntry(IEsPackFile file, IEsPackDirectory parent, ILogger? logger) : base(file, logger)
        => ParentDirectory = parent;

    protected EsPackEntry(BisBinaryReader reader, EsPackOptions options, IEsPackDirectory parent, IEsPackFile file, ILogger? logger, bool b) : base(reader, options, file, logger)
        => ParentDirectory = parent;

    public override Result Binarize(BisBinaryWriter writer, EsPackOptions options)
    {
        var name = options.Charset.GetBytes(EntryName);
        writer.Write(name.Length);
        writer.Write(name);
        return Result.Ok();
    }

    public override Result Debinarize(BisBinaryReader reader, EsPackOptions options)
    {
        var nameLength = reader.ReadInt32();
        entryName = options.Charset.GetString(reader.ReadBytes(nameLength));
        return Result.Ok();
    }

    public static EsPackEntry ReadEntry(BisBinaryReader reader, EsPackOptions options, IEsPackDirectory parent, IEsPackFile file, ILogger? logger) =>
        reader.ReadChar() switch
        {
            (char)0 => new EsPackDirectory(reader, options, parent, file, logger),
            (char)1 => new EsPackDataEntry(reader, options, parent, file, logger),
            _ => throw new IOException("Unexpected entry type")
        };
}
