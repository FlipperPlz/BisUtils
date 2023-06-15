using BisUtils.Bank.Model.Stubs;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Entry;

using Core.Family;
using FResults;

public class PboVersionEntry : PboEntry, IFamilyParent
{
    //public string FileName { get; } = "$PROPERTIES$";
    public IEnumerable<IFamilyMember> Children => Properties;
    public List<PboProperty> Properties { get; set; } = new();

    public PboVersionEntry(
        string fileName = "",
        PboEntryMime mime = PboEntryMime.Version,
        long originalSize = 0,
        long offset = 0,
        long timeStamp = 0,
        long dataSize = 0,
        List<PboProperty>? properties = null
    ) : base(fileName, mime, originalSize, offset, timeStamp, dataSize) =>
        Properties = properties ?? new List<PboProperty>();

    public PboVersionEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var result = base.Binarize(writer, options);
        if (result.IsFailed)
        {
            return result;
        }
        //TODO: Write version properties
        return result;
    }

    public override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        List<Result> results = new() { base.Debinarize(reader, options) };

        //TODO: Read version properties
        return Result.Merge(results);
    }

    public override Result Validate(PboOptions options) =>
        !(options.RequireVersionMimeOnVersion && EntryMime is not PboEntryMime.Version) &&
        !(options.RequireEmptyVersionMeta && !IsEmptyMeta()) &&
        !(options.RequireVersionNotNamed || EntryName != string.Empty) ? Result.Ok() : Result.Fail(""); //TODO: Merge results
}
