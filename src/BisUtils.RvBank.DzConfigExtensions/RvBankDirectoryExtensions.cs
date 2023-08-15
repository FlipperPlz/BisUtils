namespace BisUtils.RvBank.DzConfigExtensions;

using Core.IO;
using DzConfig;
using Extensions;
using Model.Entry;
using RvConfig.Models;
using RvConfig.Options;

public static class RvBankDirectoryExtensions
{

    public static IRvBankDataEntry? LocateConfigEntry(this IRvBankDirectory directory) =>
        directory.GetDataEntry("config.cpp") ?? directory.GetDataEntry("config.bin");

    public static IRvConfigFile? LocateConfigFile(this IRvBankDirectory directory, ParamOptions paramOptions)
    {
        var entry = LocateConfigEntry(directory);
        if (entry is null)
        {
            return null;
        }

        using var reader = new BisBinaryReader(entry.EntryData, paramOptions.Charset, true);
        return new RvConfigFile("config", reader, paramOptions, directory.Logger);
    }

    public static DzConfig? LocateAddonConfig(this IRvBankDirectory directory, ParamOptions paramOptions)
    {
        var param = LocateConfigFile(directory, paramOptions);
        return param is null ? null : new DzConfig(param);
    }

    public static IEnumerable<IRvBankDataEntry> LocateConfigEntries(this IRvBankDirectory directory, SearchOption option)
    {
        var configs = new List<IRvBankDataEntry>();
        if (directory.LocateConfigEntry() is { } configEntry)
        {
            configs.Add(configEntry);
        }

        foreach(var dir in directory.GetDirectories(option))
        {
            if (LocateConfigEntry(dir) is { } cfgEntry)
            {
                configs.Add(cfgEntry);
            }
            switch (option)
            {
                case SearchOption.TopDirectoryOnly:
                    break;
                case SearchOption.AllDirectories:
                    configs.AddRange(LocateConfigEntries(dir, option));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }

        return configs;
    }

    public static IEnumerable<IRvConfigFile> LocateConfigFiles(this IRvBankDirectory directory, ParamOptions paramOptions, SearchOption option) =>
        LocateConfigEntries(directory, option).Select(it =>
        {
            using var reader = new BisBinaryReader(it.EntryData, paramOptions.Charset, true);
            return new RvConfigFile("config", reader, paramOptions, directory.Logger);
        });

    public static IEnumerable<DzConfig> LocateAddonConfigs(this IRvBankDirectory directory, ParamOptions paramOptions,
        SearchOption option) => LocateConfigFiles(directory, paramOptions, option).Select(it => new DzConfig(it));
}
