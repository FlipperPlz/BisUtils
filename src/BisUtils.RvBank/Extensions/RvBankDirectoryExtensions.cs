namespace BisUtils.RvBank.Extensions;

using System.Diagnostics;
using Core.IO;
using Core.Parsing;
using Enumerations;
using Microsoft.Extensions.Logging;
using Model;
using Model.Entry;
using Model.Stubs;
using Options;

public static class RvBankDirectoryExtensions
{
    public static readonly Stopwatch Stopwatch = new Stopwatch();
    //ctx.PboOptions is an ObeservableCollection<IRVBankDirectory>
    public static IEnumerable<T> GetEntries<T>(this IRvBankDirectory ctx) where T : IRvBankEntry =>
        ctx.PboEntries.OfType<T>();

    public static IEnumerable<IRvBankEntry> GetEntries(this IRvBankDirectory ctx, RvBankEntryMime mime) =>
        ctx.PboEntries.Where(it => it.EntryMime == mime);

    public static IEnumerable<IRvBankEntry> GetEntries(this IRvBankDirectory ctx, string name) =>
        ctx.PboEntries.Where(it => it.EntryName == name);

    public static IRvBankEntry? GetEntry(this IRvBankDirectory ctx, string name) =>
        GetEntries(ctx, name).FirstOrDefault();

    public static IRvBankDataEntry? GetDataEntry(this IRvBankDirectory ctx, string name) =>
        (IRvBankDataEntry?) GetEntries(ctx, name).FirstOrDefault(it => it is IRvBankDataEntry);

    public static IEnumerable<IRvBankDirectory> GetDirectories(this IRvBankDirectory ctx, SearchOption option = SearchOption.TopDirectoryOnly) =>
        option switch
        {
            SearchOption.AllDirectories => EnumerateDirectoryChildren(ctx.GetEntries<IRvBankDirectory>().ToList()),
            SearchOption.TopDirectoryOnly => ctx.GetEntries<IRvBankDirectory>(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };

    public static IRvBankDirectory? GetDirectory(this IRvBankDirectory ctx, string name, SearchOption option = SearchOption.TopDirectoryOnly) =>
        GetDirectories(ctx, option).FirstOrDefault(e => e.EntryName == name);

    public static IEnumerable<IRvBankDataEntry> GetDataEntries(this IRvBankDirectory ctx, SearchOption option = SearchOption.TopDirectoryOnly) =>
        option switch
        {
            SearchOption.AllDirectories => DataEntriesFor(
                    EnumerateDirectoryChildren(ctx.GetEntries<IRvBankDirectory>().ToList()))
                .Concat(ctx.GetEntries<IRvBankDataEntry>()),
            SearchOption.TopDirectoryOnly => ctx.GetEntries<IRvBankDataEntry>(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };

    public static IRvBankDataEntry? GetDataEntry(this IRvBankDirectory ctx,
        SearchOption option = SearchOption.TopDirectoryOnly) => GetDataEntries(ctx, option).FirstOrDefault();


    public static IEnumerable<IRvBankDataEntry> GetDataEntries(this IRvBankDirectory ctx, string name,
        SearchOption option = SearchOption.TopDirectoryOnly) =>
        GetDataEntries(ctx, option).Where(it => it.EntryName == name);

    public static bool IsEmpty(this IRvBankDirectory ctx) => !ctx.PboEntries.Any();

    public static IEnumerable<IRvBankVersionEntry> GetVersionEntries(this IRvBankDirectory ctx,
        SearchOption option = SearchOption.TopDirectoryOnly) =>
        option switch
        {
            SearchOption.AllDirectories => VersionEntriesFor(
                    EnumerateDirectoryChildren(ctx.GetEntries<IRvBankDirectory>().ToList()))
                .Concat(ctx.GetEntries<IRvBankVersionEntry>()),
            SearchOption.TopDirectoryOnly => ctx.GetEntries<IRvBankVersionEntry>(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };

    public static IRvBankVersionEntry? GetVersionEntry(this IRvBankDirectory ctx, SearchOption option = SearchOption.TopDirectoryOnly) =>
        GetVersionEntries(ctx, option).FirstOrDefault();

    private static IEnumerable<IRvBankVersionEntry> VersionEntriesFor(IEnumerable<IRvBankDirectory> directories) =>
        directories.SelectMany(it => it.GetVersionEntries(SearchOption.TopDirectoryOnly));

    private static IEnumerable<IRvBankDataEntry> DataEntriesFor(IEnumerable<IRvBankDirectory> directories) =>
        directories.SelectMany(it => it.GetDataEntries(SearchOption.TopDirectoryOnly));

    private static IEnumerable<IRvBankDirectory> EnumerateDirectoryChildren(List<IRvBankDirectory> directories)
    {
        var allDirectories = new List<IRvBankDirectory>();
        foreach (var directory in directories)
        {
            allDirectories.Add(directory);
            allDirectories.AddRange(directory.GetDirectories(SearchOption.AllDirectories));
        }
        return allDirectories;
    }

    public static IRvBankVersionEntry CreateVersionEntry(this IRvBankDirectory ctx, ILogger? logger, BisBinaryReader reader,
        RvBankOptions options) =>
        new RvBankVersionEntry(reader, options, ctx.BankFile, ctx, logger);

    public static IRvBankVersionEntry CreateVersionEntry
    (
        this IRvBankDirectory ctx, ILogger? logger, string fileName = "",
        RvBankEntryMime mime = RvBankEntryMime.Version,
        uint originalSize = 0,
        ulong offset = 0,
        ulong timeStamp = 0,
        ulong dataSize = 0,
        IEnumerable<IRvBankProperty>? properties = null
    ) =>
        new RvBankVersionEntry(fileName, mime, originalSize, offset, timeStamp, dataSize, properties, ctx.BankFile, ctx, logger);

    public static IRvBankVersionEntry AddVersionEntry
    (
        this IRvBankDirectory ctx, ILogger? logger, string fileName = "",
        RvBankEntryMime mime = RvBankEntryMime.Version,
        uint originalSize = 0,
        ulong offset = 0,
        ulong timeStamp = 0,
        ulong dataSize = 0,
        IEnumerable<IRvBankProperty>? properties = null
    )
    {
        var entry = CreateVersionEntry(ctx, logger, fileName, mime, originalSize, offset, timeStamp, dataSize, properties);
        ctx.PboEntries.Add(entry);
        return entry;
    }

    public static IRvBankVersionEntry AddVersionEntry(this IRvBankDirectory ctx, ILogger? logger, BisBinaryReader reader,
        RvBankOptions options)
    {
        var entry = CreateVersionEntry(ctx, logger, reader, options);
        ctx.PboEntries.Add(entry);
        return entry;
    }

    public static IRvBankDirectory AddDirectory(this IRvBankDirectory ctx, string name, IRvBank node, ILogger? logger)
    {
        var directory = GetOrCreateDirectory(ctx, name, node, logger);
        ctx.PboEntries.Add(directory);
        return directory;
    }

    public static IRvBankDirectory GetOrCreateDirectory(this IRvBankDirectory ctx, string name, IRvBank node, ILogger? logger)
    {
        var fileName = string.Empty;
        if (!name.Contains('\\'))
        {
            goto LocateOrCreate;
        }
        if (name == string.Empty)
        {
            return ctx;
        }

        {
            var split = name.Split('\\', 2);
            name = split[0];
            fileName = split[1];
        }

        if (name.Length == 0)
        {
            return ctx;
        }

        LocateOrCreate:
        {
            IRvBankDirectory ret;
            if (GetDirectory(ctx, name) is { } i)
            {
                if (fileName.Length == 0)
                {
                    return i;
                }

                if (!ctx.PboEntries.Contains(i))
                {
                    ctx.PboEntries.Add(i);
                }

                ret = i.GetOrCreateDirectory(fileName, node, logger);

                return ret;
            }

            var directory = new RvBankDirectory(name, new List<IRvBankEntry>(), node, ctx, logger);
            ctx.PboEntries.Add(directory);
            if (fileName.Length == 0)
            {
                return directory;
            }

            ret = directory.GetOrCreateDirectory(fileName, node, logger);

            return ret;
        }

    }
    //
    // public static void AddEntry
    // (
    //     this IRVBankDirectory ctx,
    //     ILogger logger,
    //     string fileName,
    //     RVBankEntryMime mime,
    //     int originalSize,
    //     int offset,
    //     int timeStamp,
    //     int dataSize
    // ) => ctx.PboEntries.Add(new RVBankDataEntry(logger, ctx.BankFile, ctx, fileName, mime, originalSize, offset, timeStamp, dataSize));
    //
    // public static void AddEntry(this IRVBankDirectory ctx, ILogger logger, BisBinaryReader reader, RVBankOptions options) => ctx.PboEntries.Add(new RVBankDataEntry(reader, options, ctx.BankFile, ctx, logger));
    //
    // public static void AddEntry
    // (
    //     this IRVBankDirectory ctx,
    //     ILogger logger,
    //     string fileName,
    //     RVBankEntryMime mime,
    //     int offset,
    //     int timeStamp,
    //     Stream data,
    //     RVBankDataType packingMethod
    // ) => ctx.PboEntries.Add(new RVBankDataEntry(fileName, mime, offset, timeStamp, data, packingMethod, ctx.BankFile, ctx, logger));

    public static void RemoveEntry(this IRvBankDirectory ctx, IRvBankEntry entry)
    {
        ctx.PboEntries.Remove(entry);
        if (ctx.IsEmpty() && ctx != ctx.BankFile)
        {
            ctx.Delete();
        }
    }

}
