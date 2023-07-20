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

    public static IEnumerable<IRVBankVfsEntry> GetVfsEntries(this IRVBankDirectory ctx) =>
        GetEntries<IRVBankVfsEntry>(ctx);

    public static IEnumerable<IRVBankDataEntry> GetFileEntries(this IRVBankDirectory ctx) =>
        GetEntries<IRVBankDataEntry>(ctx);

    public static IEnumerable<IRVBankDirectory> GetDirectories(this IRVBankDirectory ctx) =>
        GetEntries<IRVBankDirectory>(ctx);

    public static IRVBankDirectory? GetDirectory(this IRVBankDirectory ctx, string name) =>
        GetDirectories(ctx).FirstOrDefault(e => e.EntryName == name);

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

    public static void CreateEntry
    (
        this IRVBankDirectory ctx,
        string fileName,
        RVBankEntryMime mime,
        int originalSize,
        int offset,
        int timeStamp,
        int dataSize
    ) => ctx.PboEntries.Add(new RVBankDataEntry(ctx.BankFile, ctx, fileName, mime, originalSize, offset, timeStamp, dataSize));

    public static void CreateEntry(this IRVBankDirectory ctx, BisBinaryReader reader, RVBankOptions options) => ctx.PboEntries.Add(new RVBankDataEntry(ctx.BankFile, ctx, reader, options));

    public static void CreateEntry
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
