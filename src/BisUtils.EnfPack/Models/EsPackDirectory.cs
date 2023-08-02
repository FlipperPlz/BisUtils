namespace BisUtils.EnfPack.Models;

using System.Collections.ObjectModel;
using Core.Extensions;
using Core.IO;
using Extensions;
using FResults;
using FResults.Extensions;
using Microsoft.Extensions.Logging;
using Options;

public interface IEsPackDirectory : IEsPackEntry
{
    public ObservableCollection<IEsPackEntry> PackEntries { get;  }
    public bool IsInPackRoot { get; }
}

public class EsPackDirectory : EsPackEntry, IEsPackDirectory
{
    public bool IsInPackRoot => ParentDirectory is IEsPackFile;

    private readonly ObservableCollection<IEsPackEntry> packEntries = null!;
    public ObservableCollection<IEsPackEntry> PackEntries
    {
        get => packEntries;
        init
        {
            packEntries = value;
            packEntries.CollectionChanged += (_, args) =>
            {
                OnChangesMade(this, args);
            };
        }
    }


    public EsPackDirectory(IEnumerable<IEsPackEntry>? entries, IEsPackFile file, IEsPackDirectory parent, ILogger? logger) : base(file, parent, logger) =>
        PackEntries = entries != null ? new ObservableCollection<IEsPackEntry>(entries) : new ObservableCollection<IEsPackEntry>();

    public EsPackDirectory(BisBinaryReader reader, EsPackOptions options, IEsPackDirectory parent, IEsPackFile file, ILogger? logger) : base(reader, options, parent, file, logger, true)
    {
        PackEntries = new ObservableCollection<IEsPackEntry>();
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, EsPackOptions options)
    {
        LastResult = base.Binarize(writer, options);
        writer.Write((uint) packEntries.Count);
        foreach (var entry in packEntries)
        {
            LastResult.WithoutReasons(entry.Binarize(writer, options).Reasons);
        }

        return LastResult;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, EsPackOptions options)
    {
        LastResult = base.Debinarize(reader, options);
        for (uint i = 0, entryCount = reader.ReadUInt32BE(); i < entryCount; i++)
        {
            packEntries.Add(ReadEntry(reader, options, this, PackFile, Logger));
        }

        return LastResult;
    }
    public override Result Validate(EsPackOptions options) => throw new NotImplementedException();

}
