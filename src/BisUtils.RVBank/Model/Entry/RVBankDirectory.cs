namespace BisUtils.RVBank.Model.Entry;

using System.Collections.ObjectModel;
using Core.IO;
using Enumerations;
using Options;
using FResults;
using Microsoft.Extensions.Logging;
using Stubs;

public interface IRVBankDirectory : IRVBankEntry
{
    ObservableCollection<IRVBankEntry> PboEntries { get; }
}

public class RVBankDirectory : RVBankEntry, IRVBankDirectory
{
    private readonly ObservableCollection<IRVBankEntry> pboEntries = null!;
    public ObservableCollection<IRVBankEntry> PboEntries
    {
        get => pboEntries;
        init
        {
            pboEntries = value;
            pboEntries.CollectionChanged += (_, args) =>
            {
                OnChangesMade(this, args);
            };
        }
    }

    public override RVBankEntryMime EntryMime
    {
        get => RVBankEntryMime.Decompressed;
        set => throw new NotSupportedException();
    }

    public override int OriginalSize
    {
        get => pboEntries.Sum(it => it.OriginalSize);
        set => throw new NotSupportedException();
    }

    public override int TimeStamp
    {
        get => pboEntries.Max(it => it.TimeStamp);
        set => throw new NotSupportedException();
    }

    public override int Offset
    {
        get => 0;
        set => throw new NotSupportedException();
    }

    public override int DataSize
    {
        get => pboEntries.Sum(it => it.DataSize);
        set => throw new NotSupportedException();
    }

    public RVBankDirectory(string name, IEnumerable<IRVBankEntry> entries, IRVBank file, IRVBankDirectory parent, ILogger? logger) : base(file, parent, logger)
    {
        QuietlySetName(name);
        PboEntries = new ObservableCollection<IRVBankEntry>(entries);
    }

    public override Result Validate(RVBankOptions options) => Result.Merge(pboEntries.Select(it => it.Validate(options)));

    public override Result Debinarize(BisBinaryReader reader, RVBankOptions options) =>
        throw new NotSupportedException();

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options) =>
        throw new NotSupportedException();


}
