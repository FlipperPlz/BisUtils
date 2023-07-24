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
using Microsoft.Extensions.Logging;
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

    public RVBankVersionEntry(
        ILogger logger,
        IRVBank file,
        IRVBankDirectory parent,
        string fileName = "",
        RVBankEntryMime mime = RVBankEntryMime.Version,
        uint originalSize = 0,
        uint offset = 0,
        uint timeStamp = 0,
        uint dataSize = 0,
        IEnumerable<IRVBankProperty>? properties = null
    ) : base(logger, file, parent, fileName, mime, originalSize, offset, timeStamp, dataSize) =>
        Properties = properties is { } props ? new ObservableCollection<IRVBankProperty>(props) : new ObservableCollection<IRVBankProperty>();


    public RVBankVersionEntry(ILogger logger, IRVBank file, IRVBankDirectory parent, BisBinaryReader reader, RVBankOptions options) : base(logger, file, parent, reader, options)
    {
        Properties = new ObservableCollection<IRVBankProperty>();

        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
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
        EntryMime = (RVBankEntryMime) reader.ReadInt32(); // TODO WARN/ERROR then recover
        OriginalSize = reader.ReadUInt32();
        TimeStamp = reader.ReadUInt32();
        Offset = reader.ReadUInt32();
        DataSize = reader.ReadUInt32();
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

    public new long CalculateLength(RVBankOptions options) => base.CalculateLength(options) + 1 + Properties.Sum(property =>
        2 + options.Charset.GetByteCount(property.Name) + options.Charset.GetByteCount(property.Value));

    public IRVBankProperty CreateVersionProperty(string name, string value) => new RVBankProperty(BankFile, this, name, value);

    public IRVBankProperty CreateVersionProperty(BisBinaryReader reader, RVBankOptions options) => new RVBankProperty(BankFile, this, reader, options);

    public IRVBankVersionEntry BisClone() => new RVBankVersionEntry(logger, BankFile, ParentDirectory, EntryName, EntryMime, OriginalSize, Offset,
        TimeStamp, DataSize, Properties);
}
