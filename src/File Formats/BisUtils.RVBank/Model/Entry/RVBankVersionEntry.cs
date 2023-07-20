namespace BisUtils.RVBank.Model.Entry;

using System.Collections.ObjectModel;
using Core.Cloning;
using Core.IO;
using Alerts.Errors;
using Alerts.Warnings;
using Enumerations;
using Core.Extensions;
using FResults;
using FResults.Extensions;
using Options;
using Stubs;

public interface IRVBankVersionEntry : IRVBankEntry, IBisCloneable<IRVBankVersionEntry>
{
    ObservableCollection<IRVBankProperty> Properties { get; }

    Result ReadPboProperties(BisBinaryReader reader, RVBankOptions options);
    Result WritePboProperties(BisBinaryWriter writer, RVBankOptions options);

    IRVBankProperty CreateVersionProperty(string name, string value);
    IRVBankProperty CreateVersionProperty(BisBinaryReader reader, RVBankOptions options);

}

public class RVBankVersionEntry : RVBankEntry, IRVBankVersionEntry
{
    //public string FileName { get; } = "$PROPERTIES$";
    public static readonly string[] UsedPboProperties = { "product", "prefix", "version", "encrypted", "obfuscated" };

    public RVBankVersionEntry(
        IRVBank file,
        string fileName = "",
        RVBankEntryMime mime = RVBankEntryMime.Version,
        long originalSize = 0,
        long offset = 0,
        long timeStamp = 0,
        long dataSize = 0,
        IEnumerable<IRVBankProperty>? properties = null
    ) : base(file, file, fileName, mime, originalSize, offset, timeStamp, dataSize) =>
        Properties = properties is { } props ? new ObservableCollection<IRVBankProperty>(props) : new ObservableCollection<IRVBankProperty>();

    public RVBankVersionEntry(IRVBank file, BisBinaryReader reader, RVBankOptions options) : base(file, file, reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    private readonly ObservableCollection<IRVBankProperty> properties = null!;
    public ObservableCollection<IRVBankProperty> Properties
    {
        get => properties;
        init
        {
            properties = value;
            properties.CollectionChanged += (_, args) =>
            {
                OnChangesMade(this, args);
            };
        }
    }

    public Result ReadPboProperties(BisBinaryReader reader, RVBankOptions options)
    {
        LastResult = Result.Ok();
        var property = new RVBankProperty(BankFile, this, string.Empty, string.Empty);
        Result result;
        while (!(result = property.Debinarize(reader, options)).HasError<RVBankEmptyPropertyNameError>())
        {
            LastResult.WithReasons(result.Reasons);
            if (options.RemoveBenignProperties && !UsedPboProperties.Contains(property.Name))
            {
                continue;
            }

            if (!options.AllowObfuscated && property.Name.Equals("obfuscated", StringComparison.OrdinalIgnoreCase))
            {
                throw new RVObfuscatedBankException();
            }

            if (!options.AllowEncrypted && property.Name.Equals("encrypted", StringComparison.OrdinalIgnoreCase))
            {
                throw new RVObfuscatedBankException();
            }

            Properties.Add(property.BisClone());
        }

        return LastResult;
    }

    public sealed override Result Binarize(BisBinaryWriter writer, RVBankOptions options) => LastResult = base.Binarize(writer, options).WithReasons(WritePboProperties(writer, options).Reasons);


    public sealed override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        LastResult = base.Debinarize(reader, options);
        EntryMime = (RVBankEntryMime)reader.ReadInt32(); // TODO WARN/ERROR then recover
        OriginalSize = reader.ReadInt32();
        TimeStamp = reader.ReadInt32();
        Offset = reader.ReadInt32();
        DataSize = reader.ReadInt32();
        return LastResult.WithReasons(ReadPboProperties(reader, options).Reasons);
    }


    public Result WritePboProperties(BisBinaryWriter writer, RVBankOptions options)
    {
        LastResult = Result.Ok().WithReasons(Properties.SelectMany(p => p.Binarize(writer, options).Reasons));
        writer.Write((byte)0);

        return LastResult;
    }

    public sealed override Result Validate(RVBankOptions options)
    {
        LastResult = Result.Merge(new List<Result>
        {
            EntryMime is not RVBankEntryMime.Version
                ? Result.Ok()
                    .WithWarning(new RVBankImproperMimeWarning(options.RequireVersionMimeOnVersion,
                        typeof(RVBankVersionEntry)))
                : Result.ImmutableOk(),
            !IsEmptyMeta()
                ? Result.Ok()
                    .WithWarning(new RVBankImproperMetaWarning(options.RequireEmptyVersionMeta, typeof(RVBankVersionEntry)))
                : Result.ImmutableOk(),
            EntryName != string.Empty
                ? Result.Ok()
                    .WithWarning(new RVBankNamedVersionEntryWarning(options.RequireVersionNotNamed,
                        typeof(RVBankVersionEntry)))
                : Result.ImmutableOk()
        });

        return LastResult;
    }

    public IRVBankProperty CreateVersionProperty(string name, string value) => new RVBankProperty(BankFile, this, name, value);

    public IRVBankProperty CreateVersionProperty(BisBinaryReader reader, RVBankOptions options) => new RVBankProperty(BankFile, this, reader, options);

    public IRVBankVersionEntry BisClone() => new RVBankVersionEntry(BankFile, EntryName, EntryMime, OriginalSize, Offset,
        TimeStamp, DataSize, Properties);
}
