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
        string fileName,
        RVBankEntryMime mime,
        uint originalSize,
        uint offset,
        uint timeStamp,
        uint dataSize,
        IEnumerable<IRVBankProperty>? properties,
        IRVBank file,
        IRVBankDirectory parent,
        ILogger? logger
    ) : base(fileName, mime, originalSize, offset, timeStamp, dataSize, file, parent, logger) =>
        Properties = properties is { } props ? new ObservableCollection<IRVBankProperty>(props) : new ObservableCollection<IRVBankProperty>();


    public RVBankVersionEntry(BisBinaryReader reader, RVBankOptions options, IRVBank file, IRVBankDirectory parent, ILogger? logger) : base(reader, options, file, parent, logger)
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
        var property = new RVBankProperty(string.Empty, string.Empty, BankFile, this, Logger);
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

    public sealed override Result Binarize(BisBinaryWriter writer, RVBankOptions options)
    {
        writer.WriteAsciiZ(Path, options);
        writer.Write((int)EntryMime);
        writer.Write(OriginalSize);
        writer.Write(Offset);
        writer.Write(TimeStamp);
        writer.Write(DataSize);
        return WritePboProperties(writer, options);
    }


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
    public override uint CalculateLength(RVBankOptions options) =>  (uint) (21 + options.Charset.GetByteCount(Path)) + 1 +  (uint)Properties.Sum(property =>
        2 + options.Charset.GetByteCount(property.Name) + options.Charset.GetByteCount(property.Value));

    public IRVBankProperty CreateVersionProperty(string name, string value) => new RVBankProperty(name, value, BankFile, this, Logger);

    public IRVBankProperty CreateVersionProperty(BisBinaryReader reader, RVBankOptions options) => new RVBankProperty(reader, options, BankFile, this, Logger);

    public IRVBankVersionEntry BisClone() => new RVBankVersionEntry(EntryName, EntryMime, OriginalSize, Offset,
        TimeStamp, DataSize, Properties, BankFile, ParentDirectory, Logger );
}
