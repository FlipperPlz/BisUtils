namespace BisUtils.RvBank.Model.Entry;

using System.Collections.ObjectModel;
using Core.Extensions;
using Core.IO;
using Enumerations;
using Options;
using FResults;
using Microsoft.Extensions.Logging;
using Stubs;

public interface IRvBankDirectory : IRvBankEntry
{
    ObservableCollection<IRvBankEntry> PboEntries { get; }
}

public class RvBankDirectory : RvBankEntry, IRvBankDirectory
{
    private readonly ObservableCollection<IRvBankEntry> pboEntries = null!;
    public ObservableCollection<IRvBankEntry> PboEntries
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

    public override RvBankEntryMime EntryMime
    {
        get => RvBankEntryMime.Decompressed;
        set => throw new NotSupportedException();
    }

    public override uint OriginalSize
    {
        get => pboEntries.UnsignedSum(it => it.OriginalSize);
        set => throw new NotSupportedException();
    }

    public override ulong TimeStamp
    {
        get => pboEntries.Max(it => it.TimeStamp);
        set => throw new NotSupportedException();
    }

    public override ulong Offset
    {
        get => 0;
        set => throw new NotSupportedException();
    }

    public override ulong DataSize
    {
        get => PboEntries.UnsignedSum(it => it.DataSize);
        set => throw new NotSupportedException();
    }

    public RvBankDirectory(string name, IEnumerable<IRvBankEntry> entries, IRvBank file, IRvBankDirectory parent, ILogger? logger) : base(file, parent, logger)
    {
        QuietlySetName(name);
        PboEntries = new ObservableCollection<IRvBankEntry>(entries);
    }

    public override Result Validate(RvBankOptions options) => Result.Merge(pboEntries.Select(it => it.Validate(options)));

    public override Result Debinarize(BisBinaryReader reader, RvBankOptions options) =>
        throw new NotSupportedException();

    public override Result Binarize(BisBinaryWriter writer, RvBankOptions options) =>
        throw new NotSupportedException();


}
