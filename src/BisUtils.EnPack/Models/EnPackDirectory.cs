namespace BisUtils.EnPack.Models;

using System.Collections.ObjectModel;
using BisUtils.Core.Extensions;
using BisUtils.Core.IO;
using BisUtils.EnPack.Extensions;
using FResults;
using FResults.Extensions;
using Microsoft.Extensions.Logging;
using Options;

public interface IEnPackDirectory : IEnPackEntry
{
    public ObservableCollection<IEnPackEntry> PackEntries { get;  }
    public bool IsInPackRoot { get; }
}

public class EnPackDirectory : EnPackEntry, IEnPackDirectory
{
    public bool IsInPackRoot => ParentDirectory is IEnPackFile;

    private readonly ObservableCollection<IEnPackEntry> packEntries = null!;
    public ObservableCollection<IEnPackEntry> PackEntries
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


    public EnPackDirectory(IEnumerable<IEnPackEntry>? entries, IEnPackFile file, IEnPackDirectory parent, ILogger? logger) : base(file, parent, logger) =>
        PackEntries = entries != null ? new ObservableCollection<IEnPackEntry>(entries) : new ObservableCollection<IEnPackEntry>();

    public EnPackDirectory(BisBinaryReader reader, EnPackOptions options, IEnPackDirectory parent, IEnPackFile file, ILogger? logger) : base(reader, options, parent, file, logger, true)
    {
        PackEntries = new ObservableCollection<IEnPackEntry>();
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, EnPackOptions options)
    {
        LastResult = base.Binarize(writer, options);
        writer.Write((uint) packEntries.Count);
        foreach (var entry in packEntries)
        {
            LastResult.WithoutReasons(entry.Binarize(writer, options).Reasons);
        }

        return LastResult;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, EnPackOptions options)
    {
        LastResult = base.Debinarize(reader, options);
        for (uint i = 0, entryCount = reader.ReadUInt32BE(); i < entryCount; i++)
        {
            packEntries.Add(ReadEntry(reader, options, this, PackFile, Logger));
        }

        return LastResult;
    }
    public override Result Validate(EnPackOptions options) => throw new NotImplementedException();

}
