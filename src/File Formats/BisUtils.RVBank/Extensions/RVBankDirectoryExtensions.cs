namespace BisUtils.RVBank.Extensions;

using Core.IO;
using Core.Parsing;
using Enumerations;
using Model;
using Model.Entry;
using Model.Stubs;
using Options;

public static class RVBankDirectoryExtensions
{
    public static IEnumerable<T> GetEntries<T>(this IRVBankDirectory ctx) =>
        ctx.PboEntries.OfType<T>();

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

    public static IRVBankVersionEntry CreateVersionEntry(this IRVBankDirectory ctx, BisBinaryReader reader,
        RVBankOptions options) =>
        new RVBankVersionEntry(ctx.BankFile, ctx, reader, options);

    public static IRVBankVersionEntry CreateVersionEntry
    (
        this IRVBankDirectory ctx, string fileName = "",
        RVBankEntryMime mime = RVBankEntryMime.Version,
        long originalSize = 0,
        long offset = 0,
        long timeStamp = 0,
        long dataSize = 0,
        IEnumerable<IRVBankProperty>? properties = null
    ) =>
        new RVBankVersionEntry(ctx.BankFile, ctx, fileName, mime, originalSize, offset, timeStamp, dataSize, properties);

    public static IRVBankVersionEntry AddVersionEntry
    (
        this IRVBankDirectory ctx, string fileName = "",
        RVBankEntryMime mime = RVBankEntryMime.Version,
        long originalSize = 0,
        long offset = 0,
        long timeStamp = 0,
        long dataSize = 0,
        IEnumerable<IRVBankProperty>? properties = null
    )
    {
        var entry = CreateVersionEntry(ctx, fileName, mime, originalSize, offset, timeStamp, dataSize, properties);
        ctx.PboEntries.Add(entry);
        return entry;
    }

    public static IRVBankVersionEntry AddVersionEntry(this IRVBankDirectory ctx, BisBinaryReader reader,
        RVBankOptions options)
    {
        var entry = CreateVersionEntry(ctx, reader, options);
        ctx.PboEntries.Add(entry);
        return entry;
    }

    public static IRVBankDirectory AddDirectory(this IRVBankDirectory ctx, string name, IRVBank node)
    {
        var directory = CreateDirectory(ctx, name, node);
        ctx.PboEntries.Add(directory);
        return directory;
    }


    public static IRVBankDirectory CreateDirectory(this IRVBankDirectory ctx, string name, IRVBank node)
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

            ret = i.CreateDirectory(split[1], node);

            return ret;
        }

        var directory = new RVBankDirectory(node, ctx, new List<IRVBankEntry>(), RVPathUtilities.GetFilename(split[0]));
        ctx.PboEntries.Add(directory);
        if (split[1].Length == 0)
        {
            return directory;
        }

        ret = directory.CreateDirectory(split[1], node);

        return ret;
    }

    public static void AddEntry
    (
        this IRVBankDirectory ctx,
        string fileName,
        RVBankEntryMime mime,
        int originalSize,
        int offset,
        int timeStamp,
        int dataSize
    ) => ctx.PboEntries.Add(new RVBankDataEntry(ctx.BankFile, ctx, fileName, mime, originalSize, offset, timeStamp, dataSize));

    public static void AddEntry(this IRVBankDirectory ctx, BisBinaryReader reader, RVBankOptions options) => ctx.PboEntries.Add(new RVBankDataEntry(ctx.BankFile, ctx, reader, options));

    public static void AddEntry
    (
        this IRVBankDirectory ctx,
        string fileName,
        RVBankEntryMime mime,
        int offset,
        int timeStamp,
        Stream data,
        RVBankDataType packingMethod
    ) => ctx.PboEntries.Add(new RVBankDataEntry(ctx.BankFile, ctx, fileName, mime, offset, timeStamp, data, packingMethod));

    public static void RemoveEntry(this IRVBankDirectory ctx, IRVBankEntry entry) => ctx.PboEntries.Remove(entry);

    public static void RemoveDirectory(this IRVBankDirectory ctx, IRVBankDirectory directory) => ctx.PboEntries.Remove(directory);

}
