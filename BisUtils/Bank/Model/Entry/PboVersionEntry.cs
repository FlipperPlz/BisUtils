using BisUtils.Bank.Model.Stubs;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Entry;

using Alerts.Warnings;
using Core.Binarize.Exceptions;
using Core.Family;
using FResults;

public interface IPboVersionEntry : IPboEntry, IFamilyParent
{
    IEnumerable<IFamilyMember> IFamilyParent.Children => Properties;

    List<PboProperty> Properties { get; }
}

public class PboVersionEntry : PboEntry, IPboVersionEntry
{
    //public string FileName { get; } = "$PROPERTIES$";
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
        var result = Debinarize(reader, options);
        if (result.IsFailed)
        {
            throw new DebinarizeFailedException(result.ToString());
        }
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var result = base.Binarize(writer, options);

        //TODO: Write version properties
        return result;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, PboOptions options)
    {
        List<Result> results = new() { base.Debinarize(reader, options) };

        //TODO: Read version properties
        return Result.Merge(results);
    }

    public override Result Validate(PboOptions options) => Result.Merge(new List<Result>
    {
        EntryMime is not PboEntryMime.Version
            ? Result.Ok().WithWarning(new ImproperMimeWarning(options.RequireVersionMimeOnVersion, typeof(PboVersionEntry)))
            : Result.ImmutableOk(),

        !IsEmptyMeta()
            ? Result.Ok().WithWarning(new PboImproperMetaWarning(options.RequireEmptyVersionMeta, typeof(PboVersionEntry)))
            : Result.ImmutableOk(),

        EntryName != string.Empty
            ? Result.Ok().WithWarning(new PboNamedVersionEntryWarning(options.RequireVersionNotNamed, typeof(PboVersionEntry)))
            : Result.ImmutableOk()
    });

}
