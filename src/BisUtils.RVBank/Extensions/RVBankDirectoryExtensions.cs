﻿namespace BisUtils.RVBank.Extensions;

using System.Diagnostics;
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
    public static readonly Stopwatch Stopwatch = new Stopwatch();
    //ctx.PboOptions is an ObeservableCollection<IRVBankDirectory>
    public static IEnumerable<T> GetEntries<T>(this IRVBankDirectory ctx) where T : IRVBankEntry =>
        ctx.PboEntries.OfType<T>();

    public static IEnumerable<IRVBankEntry> GetEntries(this IRVBankDirectory ctx, RVBankEntryMime mime) =>
        ctx.PboEntries.Where(it => it.EntryMime == mime);

    public static IEnumerable<IRVBankEntry> GetEntries(this IRVBankDirectory ctx, string name) =>
        ctx.PboEntries.Where(it => it.EntryName == name);

    public static IRVBankEntry? GetEntry(this IRVBankDirectory ctx, string name) =>
        GetEntries(ctx, name).FirstOrDefault();

    public static IRVBankDataEntry? GetDataEntry(this IRVBankDirectory ctx, string name) =>
        (IRVBankDataEntry?) GetEntries(ctx, name).FirstOrDefault(it => it is IRVBankDataEntry);

    public static IEnumerable<IRVBankDirectory> GetDirectories(this IRVBankDirectory ctx, SearchOption option = SearchOption.TopDirectoryOnly) =>
        option switch
        {
            SearchOption.AllDirectories => EnumerateDirectoryChildren(ctx.GetEntries<IRVBankDirectory>().ToList()),
            SearchOption.TopDirectoryOnly => ctx.GetEntries<IRVBankDirectory>(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };

    public static IRVBankDirectory? GetDirectory(this IRVBankDirectory ctx, string name, SearchOption option = SearchOption.TopDirectoryOnly) =>
        GetDirectories(ctx, option).FirstOrDefault(e => e.EntryName == name);

    public static IEnumerable<IRVBankDataEntry> GetDataEntries(this IRVBankDirectory ctx, SearchOption option = SearchOption.TopDirectoryOnly) =>
        option switch
        {
            SearchOption.AllDirectories => DataEntriesFor(
                    EnumerateDirectoryChildren(ctx.GetEntries<IRVBankDirectory>().ToList()))
                .Concat(ctx.GetEntries<IRVBankDataEntry>()),
            SearchOption.TopDirectoryOnly => ctx.GetEntries<IRVBankDataEntry>(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };

    public static IRVBankDataEntry? GetDataEntry(this IRVBankDirectory ctx,
        SearchOption option = SearchOption.TopDirectoryOnly) => GetDataEntries(ctx, option).FirstOrDefault();


    public static IEnumerable<IRVBankDataEntry> GetDataEntries(this IRVBankDirectory ctx, string name,
        SearchOption option = SearchOption.TopDirectoryOnly) =>
        GetDataEntries(ctx, option).Where(it => it.EntryName == name);

    public static bool IsEmpty(this IRVBankDirectory ctx) => !ctx.PboEntries.Any();

    public static IEnumerable<IRVBankVersionEntry> GetVersionEntries(this IRVBankDirectory ctx,
        SearchOption option = SearchOption.TopDirectoryOnly) =>
        option switch
        {
            SearchOption.AllDirectories => VersionEntriesFor(
                    EnumerateDirectoryChildren(ctx.GetEntries<IRVBankDirectory>().ToList()))
                .Concat(ctx.GetEntries<IRVBankVersionEntry>()),
            SearchOption.TopDirectoryOnly => ctx.GetEntries<IRVBankVersionEntry>(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };

    public static IRVBankVersionEntry? GetVersionEntry(this IRVBankDirectory ctx, SearchOption option = SearchOption.TopDirectoryOnly) =>
        GetVersionEntries(ctx, option).FirstOrDefault();

    private static IEnumerable<IRVBankVersionEntry> VersionEntriesFor(IEnumerable<IRVBankDirectory> directories) =>
        directories.SelectMany(it => it.GetVersionEntries(SearchOption.TopDirectoryOnly));

    private static IEnumerable<IRVBankDataEntry> DataEntriesFor(IEnumerable<IRVBankDirectory> directories) =>
        directories.SelectMany(it => it.GetDataEntries(SearchOption.TopDirectoryOnly));

    private static IEnumerable<IRVBankDirectory> EnumerateDirectoryChildren(List<IRVBankDirectory> directories)
    {
        var allDirectories = new List<IRVBankDirectory>();
        foreach (var directory in directories)
        {
            allDirectories.Add(directory);
            allDirectories.AddRange(directory.GetDirectories(SearchOption.AllDirectories));
        }
        return allDirectories;
    }

    public static IRVBankVersionEntry CreateVersionEntry(this IRVBankDirectory ctx, ILogger? logger, BisBinaryReader reader,
        RVBankOptions options) =>
        new RVBankVersionEntry(reader, options, ctx.BankFile, ctx, logger);

    public static IRVBankVersionEntry CreateVersionEntry
    (
        this IRVBankDirectory ctx, ILogger? logger, string fileName = "",
        RVBankEntryMime mime = RVBankEntryMime.Version,
        uint originalSize = 0,
        ulong offset = 0,
        ulong timeStamp = 0,
        ulong dataSize = 0,
        IEnumerable<IRVBankProperty>? properties = null
    ) =>
        new RVBankVersionEntry(fileName, mime, originalSize, offset, timeStamp, dataSize, properties, ctx.BankFile, ctx, logger);

    public static IRVBankVersionEntry AddVersionEntry
    (
        this IRVBankDirectory ctx, ILogger? logger, string fileName = "",
        RVBankEntryMime mime = RVBankEntryMime.Version,
        uint originalSize = 0,
        ulong offset = 0,
        ulong timeStamp = 0,
        ulong dataSize = 0,
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
        var directory = GetOrCreateDirectory(ctx, name, node, logger);
        ctx.PboEntries.Add(directory);
        return directory;
    }

    public static IRVBankDirectory GetOrCreateDirectory(this IRVBankDirectory ctx, string name, IRVBank node, ILogger? logger)
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
            IRVBankDirectory ret;
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

            var directory = new RVBankDirectory(name, new List<IRVBankEntry>(), node, ctx, logger);
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

    public static void RemoveEntry(this IRVBankDirectory ctx, IRVBankEntry entry)
    {
        ctx.PboEntries.Remove(entry);
        if (ctx.IsEmpty() && ctx != ctx.BankFile)
        {
            ctx.Delete();
        }
    }

}
