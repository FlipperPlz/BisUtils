namespace BisUtils.RVBank.Model.Entry;

using System.Collections.ObjectModel;
using Core.Cloning;
using Core.IO;
using Alerts.Errors;
using Alerts.Warnings;
using Enumerations;
using Core.Extensions;
using Extensions;
using FResults;
using FResults.Extensions;
using Microsoft.Extensions.Logging;
using Options;
using Stubs;

public interface IRVBankVersionEntry : IRVBankEntry, IBisCloneable<IRVBankVersionEntry>
{
    ObservableCollection<IRVBankProperty> Properties { get; }

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

    public sealed override string EntryName { get => base.EntryName; set => base.EntryName = value; }
    public sealed override RVBankEntryMime EntryMime { get => base.EntryMime; set => base.EntryMime = value; }
    public sealed override int OriginalSize { get => base.OriginalSize; set => base.OriginalSize = value; }
    public sealed override int Offset { get => base.Offset; set => base.Offset = value; }
    public sealed override int TimeStamp { get => base.TimeStamp; set => base.TimeStamp = value; }
    public sealed override int DataSize { get => base.DataSize; set => base.DataSize = value; }

    public RVBankVersionEntry(
        string fileName,
        RVBankEntryMime mime,
        int originalSize,
        int offset,
        int timeStamp,
        int dataSize,
        IEnumerable<IRVBankProperty>? properties,
        IRVBank file,
        IRVBankDirectory parent,
        ILogger? logger
    ) : base(file, parent, logger)
    {
        Properties = properties is { } props ? new ObservableCollection<IRVBankProperty>(props) : new ObservableCollection<IRVBankProperty>();
        EntryName = fileName;
        EntryMime = mime;
        OriginalSize = originalSize;
        Offset = offset;
        TimeStamp = timeStamp;
        DataSize = dataSize;

    }

    public RVBankVersionEntry(BisBinaryReader reader, RVBankOptions options, IRVBank file, IRVBankDirectory parent, ILogger? logger) : base(reader, options, file, parent, logger)
    {
        Properties = new ObservableCollection<IRVBankProperty>();

        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVBankOptions options)
    {
        LastResult = base.Debinarize(reader, options);
        ReadPboProperties(reader, options);
        return LastResult;
    }


    public sealed override Result Binarize(BisBinaryWriter writer, RVBankOptions options) =>
        Result.Merge(base.Binarize(writer, options), WritePboProperties(writer, options));


    private Result ReadPboProperties(BisBinaryReader reader, RVBankOptions options)
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
            !this.IsEmptyMeta()
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

    public sealed override int CalculateHeaderLength(RVBankOptions options) => 22 + options.Charset.GetByteCount(Path) + Properties.Sum(property =>
        2 + options.Charset.GetByteCount(property.Name) + options.Charset.GetByteCount(property.Value));

    public IRVBankProperty CreateVersionProperty(string name, string value) => new RVBankProperty(name, value, BankFile, this, Logger);

    public IRVBankProperty CreateVersionProperty(BisBinaryReader reader, RVBankOptions options) => new RVBankProperty(reader, options, BankFile, this, Logger);

    public IRVBankVersionEntry BisClone() => new RVBankVersionEntry(EntryName, EntryMime, OriginalSize, Offset,
        TimeStamp, DataSize, Properties, BankFile, ParentDirectory, Logger );
}
