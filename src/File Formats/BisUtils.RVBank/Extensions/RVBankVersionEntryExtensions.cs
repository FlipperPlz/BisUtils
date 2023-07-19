namespace BisUtils.RVBank.Extensions;

using Core.IO;
using Model.Entry;
using Options;

public static class RVBankVersionEntryExtensions
{
    public static void AddVersionEntry(this IRVBankVersionEntry ctx, BisBinaryReader reader, RVBankOptions options) => ctx.Properties.Add(ctx.CreateVersionProperty(reader, options));

    public static void AddVersionEntry(this IRVBankVersionEntry ctx, string name, string value) => ctx.Properties.Add(ctx.CreateVersionProperty(name, value));
}
