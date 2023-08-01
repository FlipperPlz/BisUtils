namespace BisUtils.RVBank.Extensions;

using Core.IO;
using Microsoft.Extensions.Logging;
using Model.Entry;
using Model.Stubs;
using Options;

public static class RVBankVersionEntryExtensions
{
    public static IRVBankProperty CreateVersionProperty(this IRVBankVersionEntry ctx, BisBinaryReader reader,
        RVBankOptions options, ILogger? logger) =>
        new RVBankProperty(reader, options, ctx.BankFile, ctx, logger);

    public static IRVBankProperty CreateVersionProperty(this IRVBankVersionEntry ctx, string name,
        string value, ILogger? logger) => new RVBankProperty(name, value, ctx.BankFile, ctx, logger);

    public static IRVBankProperty AddVersionEntry(this IRVBankVersionEntry ctx, BisBinaryReader reader, RVBankOptions options, ILogger? logger)
    {
        var prop = CreateVersionProperty(ctx, reader, options, logger);
        ctx.Properties.Add(prop);
        return prop;
    }

    public static IRVBankProperty AddVersionEntry(this IRVBankVersionEntry ctx, string name, string value, ILogger? logger)
    {
        var prop = CreateVersionProperty(ctx, name, value, logger);
        ctx.Properties.Add(prop);
        return prop;
    }

    public static IEnumerable<IRVBankProperty> GetProperties(this IRVBankVersionEntry ctx, string name) => ctx.Properties.Where(it => it.Name == name);
    public static IRVBankProperty GetOrCreateProperty(this IRVBankVersionEntry ctx, string name, string value, ILogger? logger) => GetProperties(ctx, name).FirstOrDefault() ?? AddVersionEntry(ctx, name, value, logger);

    public static IRVBankProperty SetOrCreateProperty(this IRVBankVersionEntry ctx, string name, string value, ILogger? logger)
    {
        if (GetProperties(ctx, name).FirstOrDefault() is not { } prop)
        {
            return AddVersionEntry(ctx, name, value, logger);
        }

        prop.Value = value;
        return prop;
    }


    public static IRVBankProperty? GetProperty(this IRVBankVersionEntry ctx, string name) =>
        GetProperties(ctx, name).FirstOrDefault();

    public static string? GetPropertyValue(this IRVBankVersionEntry ctx, string name) =>
        GetProperty(ctx, name)?.Value;

}
