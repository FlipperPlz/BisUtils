using BisUtils.Bank.Model.Stubs;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Entry;

using Alerts.Errors;
using Alerts.Warnings;
using Core.Binarize.Exceptions;
using Core.Cloning;
using Core.Family;
using FResults;

public interface IPboVersionEntry : IPboEntry, IFamilyParent, IBisCloneable<IPboVersionEntry>
{
    IEnumerable<IFamilyMember> IFamilyParent.Children => Properties;

    List<IPboProperty> Properties { get; }

    Result ReadPboProperties(BisBinaryReader reader, PboOptions options);

    Result WritePboProperties(BisBinaryWriter writer, PboOptions options);
}

public class PboVersionEntry : PboEntry, IPboVersionEntry
{
    //public string FileName { get; } = "$PROPERTIES$";
    public static readonly string[] UsedPboProperties = { "product", "prefix", "version", "encrypted", "obfuscated" };
    public List<IPboProperty> Properties { get; set; } = new();

    public Result ReadPboProperties(BisBinaryReader reader, PboOptions options)
    {
        var results = new List<Result>();
        var property = new PboProperty(string.Empty, string.Empty);
        var result = property.Debinarize(reader, options);
        while (!result.HasError<PboEmptyPropertyNameError>())
        {
            results.Add(result);
            if(options.RemoveBenignProperties && !UsedPboProperties.Contains(property.Name))
            {
                continue;
            }

            if (!options.AllowObfuscated && property.Name.Equals("obfuscated", StringComparison.OrdinalIgnoreCase))
            {
                throw new ObfuscatedPboException();
            }

            if (!options.AllowEncrypted && property.Name.Equals("encrypted", StringComparison.OrdinalIgnoreCase))
            {
                throw new ObfuscatedPboException();
            }

            Properties.Add(property.BisClone());
        }

        return Result.Merge(results);
    }

    public override Result Binarize(BisBinaryWriter writer, PboOptions options) =>
        Result.Merge(base.Binarize(writer, options), WritePboProperties(writer, options));

    public sealed override Result Debinarize(BisBinaryReader reader, PboOptions options) =>
        Result.Merge(base.Debinarize(reader, options), ReadPboProperties(reader, options));


    public Result WritePboProperties(BisBinaryWriter writer, PboOptions options)
    {
        var result = Result.Merge(Properties.Select(p => p.Binarize(writer, options)));
        writer.Write((byte) 0);
        return result;
    }

    public PboVersionEntry(
        IPboFile? pboFile,
        string fileName = "",
        PboEntryMime mime = PboEntryMime.Version,
        long originalSize = 0,
        long offset = 0,
        long timeStamp = 0,
        long dataSize = 0,
        List<IPboProperty>? properties = null
    ) : base(fileName, mime, originalSize, offset, timeStamp, dataSize) =>
        Properties = properties ?? new List<IPboProperty>();

    public PboVersionEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
        var result = Debinarize(reader, options);
        if (result.IsFailed)
        {
            throw new DebinarizeFailedException(result.ToString());
        }
    }


    public override Result Validate(PboOptions options) => Result.Merge(new List<Result>
    {
        EntryMime is not PboEntryMime.Version
            ? Result.Ok().WithWarning(new PboImproperMimeWarning(options.RequireVersionMimeOnVersion, typeof(PboVersionEntry)))
            : Result.ImmutableOk(),

        !IsEmptyMeta()
            ? Result.Ok().WithWarning(new PboImproperMetaWarning(options.RequireEmptyVersionMeta, typeof(PboVersionEntry)))
            : Result.ImmutableOk(),

        EntryName != string.Empty
            ? Result.Ok().WithWarning(new PboNamedVersionEntryWarning(options.RequireVersionNotNamed, typeof(PboVersionEntry)))
            : Result.ImmutableOk()
    });

    public IPboVersionEntry BisClone() => new PboVersionEntry(PboFile, EntryName, EntryMime, OriginalSize, Offset, TimeStamp, DataSize, Properties);
}
