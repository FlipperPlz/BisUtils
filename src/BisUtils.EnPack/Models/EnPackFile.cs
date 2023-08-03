namespace BisUtils.EnPack.Models;

using System.Collections.ObjectModel;
using BisUtils.Core.Binarize.Synchronization;
using BisUtils.Core.Extensions;
using BisUtils.Core.IO;
using BisUtils.Core.Parsing;
using BisUtils.EnPack.Extensions;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IEnPackFile : IEnPackDirectory, IBisSynchronizable<EnPackOptions>
{
    bool IEnPackDirectory.IsInPackRoot => true;
}

public class EnPackFile : BisSynchronizable<EnPackOptions>, IEnPackFile
{
    public IEnPackFile PackFile { get; }
    public IEnPackDirectory ParentDirectory { get; }
    public ObservableCollection<IEnPackEntry> PackEntries { get; }
    public virtual string Path => $"{ParentDirectory.Path}\\{EntryName}";
    public virtual string AbsolutePath => $"{ParentDirectory.AbsolutePath}\\{EntryName}";

    private string entryName;
    public virtual string EntryName
    {
        get => RVPathUtilities.NormalizePboPath(entryName);
        set
        {
            OnChangesMade(this, EventArgs.Empty);
            entryName = value;
        }
    }

    public EnPackFile(string name, IEnumerable<IEnPackEntry>? entries, Stream? syncTo, ILogger? logger) : base(syncTo, logger)
    {
        entryName = name;
        PackFile = this;
        ParentDirectory = this;
        PackEntries = entries != null ? new ObservableCollection<IEnPackEntry>(entries) : new ObservableCollection<IEnPackEntry>();
    }

    public EnPackFile(string name, BisBinaryReader reader, EnPackOptions options, Stream? syncTo, ILogger? logger) : base(syncTo, logger)
    {
        entryName = name;
        PackFile = this;
        ParentDirectory = this;
        PackEntries = new ObservableCollection<IEnPackEntry>();
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public EnPackFile(string fileName, Stream buffer, EnPackOptions options, Stream? syncTo, ILogger logger) : this(fileName, (IEnumerable<EnPackEntry>) null!, syncTo, logger)
    {
        using var reader = new BisBinaryReader(buffer, options.Charset);
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public EnPackFile(string path, EnPackOptions options, Stream? syncTo, ILogger logger) : this(System.IO.Path.GetFileNameWithoutExtension(path), (IEnumerable<EnPackEntry>) null!, syncTo, logger)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BisBinaryReader(stream, options.Charset);
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, EnPackOptions options) =>
        throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, EnPackOptions options)
    {
        reader.ScanUntil(options, "FORM");
        reader.ScanUntil(options, "PAC1");
        reader.ScanUntil(options, "HEAD");
        reader.ScanUntil(options, "DATA");
        reader.ScanUntil(options, "DATA");
        var fileSize = reader.ReadUInt32BE();
        var eof = reader.BaseStream.Position + fileSize;
        reader.BaseStream.Seek(2, SeekOrigin.Current); // Flags maybe
        do
        {
            PackEntries.Add(EnPackEntry.ReadEntry(reader, options, this, this, Logger));
        } while (reader.BaseStream.Position < eof);

        return Result.Ok();
    }


    public override Result Validate(EnPackOptions options) =>
        throw new NotImplementedException();



}
