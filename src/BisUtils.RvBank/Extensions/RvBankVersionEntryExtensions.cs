namespace BisUtils.RvBank.Extensions;

using Core.IO;
using Microsoft.Extensions.Logging;
using Model.Entry;
using Model.Stubs;
using Options;

public static class RvBankVersionEntryExtensions
{
    public static IRvBankProperty CreateVersionProperty(this IRvBankVersionEntry ctx, BisBinaryReader reader,
        RvBankOptions options, ILogger? logger) =>
        new RvBankProperty(reader, options, ctx.BankFile, ctx, logger);

    public static IRvBankProperty CreateVersionProperty(this IRvBankVersionEntry ctx, string name,
        string value, ILogger? logger) => new RvBankProperty(name, value, ctx.BankFile, ctx, logger);

    public static IRvBankProperty AddVersionEntry(this IRvBankVersionEntry ctx, BisBinaryReader reader, RvBankOptions options, ILogger? logger)
    {
        var prop = CreateVersionProperty(ctx, reader, options, logger);
        ctx.Properties.Add(prop);
        return prop;
    }

    public static IRvBankProperty AddVersionEntry(this IRvBankVersionEntry ctx, string name, string value, ILogger? logger)
    {
        var prop = CreateVersionProperty(ctx, name, value, logger);
        ctx.Properties.Add(prop);
        return prop;
    }

    public static IEnumerable<IRvBankProperty> GetProperties(this IRvBankVersionEntry ctx, string name) => ctx.Properties.Where(it => it.Name == name);
    public static IRvBankProperty GetOrCreateProperty(this IRvBankVersionEntry ctx, string name, string value, ILogger? logger) => GetProperties(ctx, name).FirstOrDefault() ?? AddVersionEntry(ctx, name, value, logger);

    public static IRvBankProperty SetOrCreateProperty(this IRvBankVersionEntry ctx, string name, string value, ILogger? logger)
    {
        if (GetProperties(ctx, name).FirstOrDefault() is not { } prop)
        {
            return AddVersionEntry(ctx, name, value, logger);
        }

        prop.Value = value;
        return prop;
    }


    public static IRvBankProperty? GetProperty(this IRvBankVersionEntry ctx, string name) =>
        GetProperties(ctx, name).FirstOrDefault();

    public static string? GetPropertyValue(this IRvBankVersionEntry ctx, string name) =>
        GetProperty(ctx, name)?.Value;

}
