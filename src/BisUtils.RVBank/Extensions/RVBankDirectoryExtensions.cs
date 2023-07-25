namespace BisUtils.RVBank.Extensions;

using Core.IO;
using Core.Parsing;
using Enumerations;
using Microsoft.Extensions.Logging;
using Model;
using Model.Entry;
using Model.Stubs;
using Options;

public static class RVBankDirectoryExtensions
{
    public static IEnumerable<T> GetEntries<T>(this IRVBankDirectory ctx) =>
        ctx.PboEntries.OfType<T>();

    public static IEnumerable<T> GetEntries<T>(this IRVBankDirectory ctx, string name) where T: IRVBankEntry=>
        GetEntries<T>(ctx).Where(it => it.EntryName == name);

    public static IEnumerable<IRVBankVfsEntry> GetVfsEntries(this IRVBankDirectory ctx, bool recursive = false) =>
        GetEntries<IRVBankVfsEntry>(ctx);

    public static IEnumerable<IRVBankDataEntry> GetFileEntries(this IRVBankDirectory ctx, bool recursive = false) =>
        !recursive ? GetEntries<IRVBankDataEntry>(ctx) : GetDirectories(ctx).SelectMany(it => it.GetFileEntries(recursive))
            .Concat(GetEntries<IRVBankDataEntry>(ctx));


    public static IEnumerable<IRVBankDirectory> GetDirectories(this IRVBankDirectory ctx) =>
        GetEntries<IRVBankDirectory>(ctx);

    public static IEnumerable<IRVBankVersionEntry> GetVersionEntries(this IRVBankDirectory ctx) =>
        GetEntries<IRVBankVersionEntry>(ctx);

    public static IRVBankVersionEntry? GetVersionEntry(this IRVBankDirectory ctx) =>
        GetVersionEntries(ctx).FirstOrDefault();

    public static IRVBankDirectory? GetDirectory(this IRVBankDirectory ctx, string name) =>
        GetDirectories(ctx).FirstOrDefault(e => e.EntryName == name);

    public static IRVBankVersionEntry CreateVersionEntry(this IRVBankDirectory ctx, ILogger? logger, BisBinaryReader reader,
        RVBankOptions options) =>
        new RVBankVersionEntry(reader, options, ctx.BankFile, ctx, logger);

    public static IRVBankVersionEntry CreateVersionEntry
    (
        this IRVBankDirectory ctx, ILogger? logger, string fileName = "",
        RVBankEntryMime mime = RVBankEntryMime.Version,
        uint originalSize = 0,
        uint offset = 0,
        uint timeStamp = 0,
        uint dataSize = 0,
        IEnumerable<IRVBankProperty>? properties = null
    ) =>
        new RVBankVersionEntry(fileName, mime, originalSize, offset, timeStamp, dataSize, properties, ctx.BankFile, ctx, logger);

    public static IRVBankVersionEntry AddVersionEntry
    (
        this IRVBankDirectory ctx, ILogger? logger, string fileName = "",
        RVBankEntryMime mime = RVBankEntryMime.Version,
        uint originalSize = 0,
        uint offset = 0,
        uint timeStamp = 0,
        uint dataSize = 0,
        IEnumerable<IRVBankProperty>? properties = null
    )
    {
        var entry = CreateVersionEntry(ctx, logger, fileName, mime, originalSize, offset, timeStamp, dataSize, properties);
        ctx.PboEntries.Add(entry);
        return entry;
    }

    public static IRVBankVersionEntry AddVersionEntry(this IRVBankDirectory ctx, ILogger? logger, BisBinaryReader reader,
        RVBankOptions options)
    {
        var entry = CreateVersionEntry(ctx, logger, reader, options);
        ctx.PboEntries.Add(entry);
        return entry;
    }

    public static IRVBankDirectory AddDirectory(this IRVBankDirectory ctx, string name, IRVBank node, ILogger? logger)
    {
        var directory = CreateDirectory(ctx, name, node, logger);
        ctx.PboEntries.Add(directory);
        return directory;
    }

    public static IEnumerable<IRVBankDataEntry> GetFileEntries(this IRVBankDirectory ctx, string name, bool recursive = false) =>
        GetFileEntries(ctx, recursive).Where(e => e.EntryName.Equals(name, StringComparison.OrdinalIgnoreCase));

    public static IRVBankDataEntry? GetFileEntry(this IRVBankDirectory ctx, string name, bool recursive = false) =>
        GetFileEntries(ctx, name, recursive).FirstOrDefault();

    public static IRVBankDirectory CreateDirectory(this IRVBankDirectory ctx, string name, IRVBank node, ILogger? logger)
    {
        var split = name.Split('\\', 2);
        IRVBankDirectory ret;

        if (split[0].Length == 0)
        {
            return ctx;
        }

        if (GetDirectory(ctx, split[0]) is { } i)
        {
            if (split[1].Length == 0)
            {
                return i;
            }

            if (!ctx.PboEntries.Contains(i))
            {
                ctx.PboEntries.Add(i);
            }

            ret = i.CreateDirectory(split[1], node, logger);

            return ret;
        }

        var directory = new RVBankDirectory(new List<IRVBankEntry>(), RVPathUtilities.GetFilename(split[0]), node, ctx, logger);
        ctx.PboEntries.Add(directory);
        if (split[1].Length == 0)
        {
            return directory;
        }

        ret = directory.CreateDirectory(split[1], node, logger);

        return ret;
    }

    public static void AddEntry
    (
        this IRVBankDirectory ctx,
        ILogger logger,
        string fileName,
        RVBankEntryMime mime,
        uint originalSize,
        uint offset,
        uint timeStamp,
        uint dataSize
    ) => ctx.PboEntries.Add(new RVBankDataEntry(logger, ctx.BankFile, ctx, fileName, mime, originalSize, offset, timeStamp, dataSize));

    public static void AddEntry(this IRVBankDirectory ctx, ILogger logger, BisBinaryReader reader, RVBankOptions options) => ctx.PboEntries.Add(new RVBankDataEntry(reader, options, ctx.BankFile, ctx, logger));

    public static void AddEntry
    (
        this IRVBankDirectory ctx,
        ILogger logger,
        string fileName,
        RVBankEntryMime mime,
        uint offset,
        uint timeStamp,
        Stream data,
        RVBankDataType packingMethod
    ) => ctx.PboEntries.Add(new RVBankDataEntry(fileName, mime, offset, timeStamp, data, packingMethod, ctx.BankFile, ctx, logger));

    public static void RemoveEntry(this IRVBankDirectory ctx, IRVBankEntry entry) => ctx.PboEntries.Remove(entry);

    public static void RemoveDirectory(this IRVBankDirectory ctx, IRVBankDirectory directory) => ctx.PboEntries.Remove(directory);

}
