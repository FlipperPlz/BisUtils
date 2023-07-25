namespace BisUtils.RVBank.Model.Stubs;

using System.Collections.ObjectModel;
using Core.IO;
using Enumerations;
using Entry;
using Extensions;
using FResults;
using FResults.Extensions;
using Microsoft.Extensions.Logging;
using Options;

public interface IRVBankDirectory : IRVBankEntry
{
    ObservableCollection<IRVBankEntry> PboEntries { get; }

    uint IRVBankEntry.OriginalSize => (uint) PboEntries.Sum(e => e.OriginalSize);

    uint IRVBankEntry.Offset => throw new NotSupportedException();

    uint IRVBankEntry.TimeStamp
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    uint IRVBankEntry.DataSize => (uint) PboEntries.Sum(e => e.DataSize);

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
        IEnumerable<IRVBankEntry> entries,
        string directoryName,
        IRVBank file,
        IRVBankDirectory parent,
        ILogger? logger
    ) : base(directoryName, file, parent, logger) =>
        PboEntries = new ObservableCollection<IRVBankEntry>(entries);

    protected RVBankDirectory(BisBinaryReader reader, RVBankOptions options, IRVBank file, IRVBankDirectory parent, ILogger? logger) : base(reader, options, file, parent, logger) =>
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

    public uint CalculateLength(RVBankOptions options) => 0;
}
