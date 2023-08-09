namespace BisUtils.RvBank.Model.Entry;

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

public interface IRvBankVersionEntry : IRvBankEntry, IBisCloneable<IRvBankVersionEntry>
{
    ObservableCollection<IRvBankProperty> Properties { get; }

}

public class RvBankVersionEntry : RvBankEntry, IRvBankVersionEntry
{
    //public string FileName { get; } = "$PROPERTIES$";
    public static readonly string[] UsedPboProperties = { "product", "prefix", "version", "encrypted", "obfuscated" };


    private readonly ObservableCollection<IRvBankProperty> properties = null!;
    public ObservableCollection<IRvBankProperty> Properties
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
    public sealed override RvBankEntryMime EntryMime { get => base.EntryMime; set => base.EntryMime = value; }
    public sealed override uint OriginalSize { get => base.OriginalSize; set => base.OriginalSize = value; }
    public sealed override ulong Offset { get => base.Offset; set => base.Offset = value; }
    public sealed override ulong TimeStamp { get => base.TimeStamp; set => base.TimeStamp = value; }
    public sealed override ulong DataSize { get => base.DataSize; set => base.DataSize = value; }

    public RvBankVersionEntry(
        string fileName,
        RvBankEntryMime mime,
        uint originalSize,
        ulong offset,
        ulong timeStamp,
        ulong dataSize,
        IEnumerable<IRvBankProperty>? properties,
        IRvBank file,
        IRvBankDirectory parent,
        ILogger? logger
    ) : base(file, parent, logger)
    {
        Properties = properties is { } props ? new ObservableCollection<IRvBankProperty>(props) : new ObservableCollection<IRvBankProperty>();
        EntryName = fileName;
        EntryMime = mime;
        OriginalSize = originalSize;
        Offset = offset;
        TimeStamp = timeStamp;
        DataSize = dataSize;

    }

    public RvBankVersionEntry(BisBinaryReader reader, RvBankOptions options, IRvBank file, IRvBankDirectory parent, ILogger? logger) : base(reader, options, file, parent, logger)
    {
        Properties = new ObservableCollection<IRvBankProperty>();

        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RvBankOptions options)
    {
        LastResult = base.Debinarize(reader, options);
        ReadPboProperties(reader, options);
        return LastResult;
    }


    public sealed override Result Binarize(BisBinaryWriter writer, RvBankOptions options) =>
        Result.Merge(base.Binarize(writer, options), WritePboProperties(writer, options));


    private Result ReadPboProperties(BisBinaryReader reader, RvBankOptions options)
    {
        LastResult = Result.Ok();
        var property = new RvBankProperty(string.Empty, string.Empty, BankFile, this, Logger);
        Result result;
        while (!(result = property.Debinarize(reader, options)).HasError<RvBankEmptyPropertyNameError>())
        {
            LastResult.WithReasons(result.Reasons);
            if (options.RemoveBenignProperties && !UsedPboProperties.Contains(property.Name))
            {
                continue;
            }

            if (!options.AllowObfuscated && property.Name.Equals("obfuscated", StringComparison.OrdinalIgnoreCase))
            {
                throw new RvObfuscatedBankException();
            }

            if (!options.AllowEncrypted && property.Name.Equals("encrypted", StringComparison.OrdinalIgnoreCase))
            {
                throw new RvObfuscatedBankException();
            }

            Properties.Add(property.BisClone());
        }

        return LastResult;
    }


    public Result WritePboProperties(BisBinaryWriter writer, RvBankOptions options)
    {
        LastResult = Result.Ok().WithReasons(Properties.SelectMany(p => p.Binarize(writer, options).Reasons));
        writer.Write((byte)0);

        return LastResult;
    }

    public sealed override Result Validate(RvBankOptions options)
    {
        LastResult = Result.Merge(new List<Result>
        {
            EntryMime is not RvBankEntryMime.Version
                ? Result.Ok()
                    .WithWarning(new RvBankImproperMimeWarning(options.RequireVersionMimeOnVersion,
                        typeof(RvBankVersionEntry)))
                : Result.ImmutableOk(),
            !this.IsEmptyMeta()
                ? Result.Ok()
                    .WithWarning(new RvBankImproperMetaWarning(options.RequireEmptyVersionMeta, typeof(RvBankVersionEntry)))
                : Result.ImmutableOk(),
            EntryName != string.Empty
                ? Result.Ok()
                    .WithWarning(new RvBankNamedVersionEntryWarning(options.RequireVersionNotNamed,
                        typeof(RvBankVersionEntry)))
                : Result.ImmutableOk()
        });

        return LastResult;
    }

    public sealed override int CalculateHeaderLength(RvBankOptions options) => 22 + options.Charset.GetByteCount(Path) + Properties.Sum(property =>
        2 + options.Charset.GetByteCount(property.Name) + options.Charset.GetByteCount(property.Value));

    public IRvBankProperty CreateVersionProperty(string name, string value) => new RvBankProperty(name, value, BankFile, this, Logger);

    public IRvBankProperty CreateVersionProperty(BisBinaryReader reader, RvBankOptions options) => new RvBankProperty(reader, options, BankFile, this, Logger);

    public IRvBankVersionEntry BisClone() => new RvBankVersionEntry(EntryName, EntryMime, OriginalSize, Offset,
        TimeStamp, DataSize, Properties, BankFile, ParentDirectory, Logger );
}
