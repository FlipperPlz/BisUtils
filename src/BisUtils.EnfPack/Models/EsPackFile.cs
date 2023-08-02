namespace BisUtils.EnfPack.Models;

using System.Collections.ObjectModel;
using Core.Binarize.Synchronization;
using Core.Extensions;
using Core.IO;
using Core.Parsing;
using Extensions;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IEsPackFile : IEsPackDirectory, IBisSynchronizable<EsPackOptions>
{
    bool IEsPackDirectory.IsInPackRoot => true;
}

public class EsPackFile : BisSynchronizable<EsPackOptions>, IEsPackFile
{
    public IEsPackFile PackFile { get; }
    public IEsPackDirectory ParentDirectory { get; }
    public ObservableCollection<IEsPackEntry> PackEntries { get; }
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

    public EsPackFile(string name, IEnumerable<IEsPackEntry>? entries, Stream? syncTo, ILogger? logger) : base(syncTo, logger)
    {
        entryName = name;
        PackFile = this;
        ParentDirectory = this;
        PackEntries = entries != null ? new ObservableCollection<IEsPackEntry>(entries) : new ObservableCollection<IEsPackEntry>();
    }

    public EsPackFile(string name, BisBinaryReader reader, EsPackOptions options, Stream? syncTo, ILogger? logger) : base(syncTo, logger)
    {
        entryName = name;
        PackFile = this;
        ParentDirectory = this;
        PackEntries = new ObservableCollection<IEsPackEntry>();
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public EsPackFile(string fileName, Stream buffer, EsPackOptions options, Stream? syncTo, ILogger logger) : this(fileName, (IEnumerable<EsPackEntry>) null!, syncTo, logger)
    {
        using var reader = new BisBinaryReader(buffer, options.Charset);
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public EsPackFile(string path, EsPackOptions options, Stream? syncTo, ILogger logger) : this(System.IO.Path.GetFileNameWithoutExtension(path), (IEnumerable<EsPackEntry>) null!, syncTo, logger)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BisBinaryReader(stream, options.Charset);
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, EsPackOptions options) =>
        throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, EsPackOptions options)
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
            PackEntries.Add(EsPackEntry.ReadEntry(reader, options, this, this, Logger));
        } while (reader.BaseStream.Position < eof);

        return Result.Ok();
    }


    public override Result Validate(EsPackOptions options) =>
        throw new NotImplementedException();



}
