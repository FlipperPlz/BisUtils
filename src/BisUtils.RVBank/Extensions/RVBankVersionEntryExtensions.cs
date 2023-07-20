namespace BisUtils.RVBank.Extensions;

using Core.IO;
using Model.Entry;
using Model.Stubs;
using Options;

public static class RVBankVersionEntryExtensions
{
    public static IRVBankProperty AddVersionEntry(this IRVBankVersionEntry ctx, BisBinaryReader reader, RVBankOptions options)
    {
        var prop = ctx.CreateVersionProperty(reader, options);
        ctx.Properties.Add(prop);
        return prop;
    }

    public static IRVBankProperty AddVersionEntry(this IRVBankVersionEntry ctx, string name, string value)
    {
        var prop = ctx.CreateVersionProperty(name, value);
        ctx.Properties.Add(prop);
        return prop;
    }

    public static IEnumerable<IRVBankProperty> GetProperties(this IRVBankVersionEntry ctx, string name) => ctx.Properties.Where(it => it.Name == name);
    public static IRVBankProperty GetOrCreateProperty(this IRVBankVersionEntry ctx, string name, string value) => GetProperties(ctx, name).FirstOrDefault() ?? AddVersionEntry(ctx, name, value);

    public static IRVBankProperty SetOrCreateProperty(this IRVBankVersionEntry ctx, string name, string value)
    {
        if (GetProperties(ctx, name).FirstOrDefault() is not { } prop)
        {
            return AddVersionEntry(ctx, name, value);
        }

        prop.Value = value;
        return prop;
    }


    public static IRVBankProperty? GetProperty(this IRVBankVersionEntry ctx, string name) =>
        GetProperties(ctx, name).FirstOrDefault();

    public static string? GetPropertyValue(this IRVBankVersionEntry ctx, string name) =>
        GetProperty(ctx, name)?.Value;

}
