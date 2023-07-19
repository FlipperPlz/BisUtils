namespace BisUtils.RVBank.Model.Stubs;

using Core.IO;
using Enumerations;
using Entry;
using FResults;
using FResults.Extensions;
using Options;

public interface IRVBankDirectory : IRVBankEntry
{
    List<IRVBankEntry> PboEntries { get; }

    IEnumerable<IRVBankDataEntry> FileEntries { get; }

    IEnumerable<IRVBankDirectory> Directories { get; }

    IEnumerable<IRVBankVfsEntry> VfsEntries => PboEntries.OfType<IRVBankVfsEntry>().ToList();

    int IRVBankEntry.OriginalSize => PboEntries.Sum(e => e.OriginalSize);

    int IRVBankEntry.Offset => throw new NotSupportedException();

    int IRVBankEntry.TimeStamp => throw new NotSupportedException();

    int IRVBankEntry.DataSize => PboEntries.Sum(e => e.DataSize);

    RVBankEntryMime IRVBankEntry.EntryMime => throw new NotSupportedException();
}

public class RVBankDirectory : RVBankVfsEntry, IRVBankDirectory
{
    //TODO call ChangesMade on entry add/remove
    public List<IRVBankEntry> PboEntries { get; set; } = new();
    public IEnumerable<IRVBankDataEntry> FileEntries => PboEntries.OfType<IRVBankDataEntry>();
    public IEnumerable<IRVBankDirectory> Directories => PboEntries.OfType<IRVBankDirectory>();

    public RVBankDirectory(
        IRVBank file,
        IRVBankDirectory parent,
        List<IRVBankEntry> entries,
        string directoryName
    ) : base(file, parent, directoryName) => PboEntries = entries;

    protected RVBankDirectory(IRVBank file, IRVBankDirectory parent, BisBinaryReader reader, RVBankOptions options) : base(file, parent, reader, options)
    {
    }



    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options) =>
        LastResult = Result.Ok().WithReasons(PboEntries.SelectMany(e => e.Binarize(writer, options).Reasons));

    public override Result Debinarize(BisBinaryReader reader, RVBankOptions options) =>
        throw new NotSupportedException();

    public override Result Validate(RVBankOptions options) =>
        LastResult = Result.Ok().WithReasons(PboEntries.SelectMany(e => e.Validate(options).Reasons));

}
