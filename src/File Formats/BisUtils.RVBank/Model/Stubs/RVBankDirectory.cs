namespace BisUtils.RVBank.Model.Stubs;

using System.Collections.ObjectModel;
using Core.IO;
using Enumerations;
using Entry;
using Extensions;
using FResults;
using FResults.Extensions;
using Options;

public interface IRVBankDirectory : IRVBankEntry
{
    ObservableCollection<IRVBankEntry> PboEntries { get; }

    long IRVBankEntry.OriginalSize => PboEntries.Sum(e => e.OriginalSize);

    long IRVBankEntry.Offset => throw new NotSupportedException();

    long IRVBankEntry.TimeStamp => throw new NotSupportedException();

    long IRVBankEntry.DataSize => PboEntries.Sum(e => e.DataSize);

    RVBankEntryMime IRVBankEntry.EntryMime => throw new NotSupportedException();
}

public class RVBankDirectory : RVBankVfsEntry, IRVBankDirectory
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

    public IEnumerable<IRVBankDataEntry> FileEntries => PboEntries.OfType<IRVBankDataEntry>();
    public IEnumerable<IRVBankDirectory> Directories => PboEntries.OfType<IRVBankDirectory>();

    public RVBankDirectory(
        IRVBank file,
        IRVBankDirectory parent,
        IEnumerable<IRVBankEntry> entries,
        string directoryName
    ) : base(file, parent, directoryName) =>
        PboEntries = new ObservableCollection<IRVBankEntry>(entries);

    protected RVBankDirectory(IRVBank file, IRVBankDirectory parent, BisBinaryReader reader, RVBankOptions options) : base(file, parent, reader, options) =>
        PboEntries = new ObservableCollection<IRVBankEntry>();

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options) =>
        LastResult = Result.Ok().WithReasons(PboEntries.SelectMany(e => e.Binarize(writer, options).Reasons));

    public override Result Debinarize(BisBinaryReader reader, RVBankOptions options) =>
        throw new NotSupportedException();

    public override Result Validate(RVBankOptions options) =>
        LastResult = Result.Ok().WithReasons(PboEntries.SelectMany(e => e.Validate(options).Reasons));

    public void Move(IRVBankDirectory destination)
    {
        if (destination.BankFile != BankFile)
        {
            throw new IOException("Cannot move this entry to a directory outside of the current pbo.");
        }

        ParentDirectory.RemoveDirectory(this);
        ParentDirectory = destination;
        OnChangesMade(this, EventArgs.Empty);
    }
}
