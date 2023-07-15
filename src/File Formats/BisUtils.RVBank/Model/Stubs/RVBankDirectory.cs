namespace BisUtils.RVBank.Model.Stubs;

using BisUtils.Core.Family;
using BisUtils.Core.IO;
using BisUtils.RVBank.Enumerations;
using BisUtils.RVBank.Model.Entry;
using Core.Parsing;
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

    IRVBankDirectory? GetDirectory(string name);

    IRVBankDirectory CreateDirectory(string name, IRVBank? node);
}

public class RVBankDirectory : RVBankVfsEntry, IRVBankDirectory
{

    public List<IRVBankEntry> PboEntries { get; set; } = new();
    public IEnumerable<IRVBankDataEntry> FileEntries => PboEntries.OfType<IRVBankDataEntry>();
    public IEnumerable<IRVBankDirectory> Directories => PboEntries.OfType<IRVBankDirectory>();

    public RVBankDirectory(
        IRVBank file,
        IRVBankDirectory? parent,
        List<IRVBankEntry> entries,
        string directoryName
    ) : base(file, parent, directoryName) => PboEntries = entries;

    protected RVBankDirectory(IRVBank file, IRVBankDirectory? parent, BisBinaryReader reader, RVBankOptions options) : base(file, parent, reader, options)
    {
    }

    public IRVBankDirectory? GetDirectory(string name) =>
        Directories.FirstOrDefault(e => e.EntryName == name);

    public IRVBankDirectory CreateDirectory(string name, IRVBank? node)
    {
        var split = name.Split('\\', 2);
        IRVBankDirectory ret;

        if (split[0].Length == 0)
        {
            return this;
        }

        if (GetDirectory(split[0]) is { } i)
        {
            if (split[1].Length == 0)
            {
                return i;
            }

            if (!PboEntries.Contains(i))
            {
                PboEntries.Add(i);
            }

            ret = i.CreateDirectory(split[1], node);

            return ret;
        }

        var directory = new RVBankDirectory(node, this, new List<IRVBankEntry>(), RVPathUtilities.GetFilename(split[0]));
        PboEntries.Add(directory);
        if (split[1].Length == 0)
        {
            return directory;
        }

        ret = directory.CreateDirectory(split[1], node);

        return ret;
    }

    public override Result Binarize(BisBinaryWriter writer, RVBankOptions options) =>
        LastResult = Result.Ok().WithReasons(PboEntries.SelectMany(e => e.Binarize(writer, options).Reasons));

    public override Result Debinarize(BisBinaryReader reader, RVBankOptions options) =>
        throw new NotSupportedException();

    public override Result Validate(RVBankOptions options) =>
        LastResult = Result.Ok().WithReasons(PboEntries.SelectMany(e => e.Validate(options).Reasons));
}
