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

    public static IRVBankDirectory? GetDirectory(this IRVBankDirectory ctx, string name) =>
        ctx.Directories.FirstOrDefault(e => e.EntryName == name);

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
