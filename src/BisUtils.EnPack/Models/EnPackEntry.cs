namespace BisUtils.EnPack.Models;

using BisUtils.Core.Binarize.Synchronization;
using BisUtils.Core.IO;
using BisUtils.Core.Parsing;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IEnPackEntry : IBisSynchronizableElement<EnPackOptions>, IEnPackElement
{
    public IEnPackDirectory ParentDirectory { get; }
    public string EntryName { get; set; }
    public string Path { get; }
    public string AbsolutePath { get; }
}

public abstract class EnPackEntry : EnPackElement, IEnPackEntry
{
    public IEnPackDirectory ParentDirectory { get; private set; }
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

    protected EnPackEntry(IEnPackFile file, IEnPackDirectory parent, ILogger? logger) : base(file, logger)
        => ParentDirectory = parent;

    protected EnPackEntry(BisBinaryReader reader, EnPackOptions options, IEnPackDirectory parent, IEnPackFile file, ILogger? logger, bool b) : base(reader, options, file, logger)
        => ParentDirectory = parent;

    public override Result Binarize(BisBinaryWriter writer, EnPackOptions options)
    {
        var name = options.Charset.GetBytes(EntryName);
        writer.Write(name.Length);
        writer.Write(name);
        return Result.Ok();
    }

    public override Result Debinarize(BisBinaryReader reader, EnPackOptions options)
    {
        var nameLength = reader.ReadInt32();
        entryName = options.Charset.GetString(reader.ReadBytes(nameLength));
        return Result.Ok();
    }

    public static EnPackEntry ReadEntry(BisBinaryReader reader, EnPackOptions options, IEnPackDirectory parent, IEnPackFile file, ILogger? logger) =>
        reader.ReadChar() switch
        {
            (char)0 => new EnPackDirectory(reader, options, parent, file, logger),
            (char)1 => new EnPackDataEntry(reader, options, parent, file, logger),
            _ => throw new IOException("Unexpected entry type")
        };
}
