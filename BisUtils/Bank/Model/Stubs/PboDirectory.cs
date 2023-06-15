using BisUtils.Core.Family;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Stubs;

using FResults;

public class PboDirectory : PboVFSEntry, IFamilyParent
{
    public List<PboEntry> PboEntries { get; set; } = new();

    public IEnumerable<PboVFSEntry> VfsEntries => PboEntries.OfType<PboVFSEntry>().ToList();
    public IEnumerable<IFamilyMember> Children => VfsEntries;

    public PboDirectory(List<PboEntry> entries, string directoryName) : base(directoryName) => PboEntries = entries;

    public PboDirectory(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options) =>
        Result.Merge(new List<Result> { Result.ImmutableOk() }.Concat(PboEntries.Select(e => e.Binarize(writer, options))));

    public override Result Debinarize(BisBinaryReader reader, PboOptions options) =>
        throw new NotSupportedException();

    public override Result Validate(PboOptions options) =>
        Result.Merge(PboEntries.Select(e => e.Validate(options)));
}
