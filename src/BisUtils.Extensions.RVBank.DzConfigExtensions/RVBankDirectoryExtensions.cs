namespace BisUtils.Extensions.RVBank.DzConfigExtensions;

using BisUtils.RVBank.Extensions;
using BisUtils.RVBank.Model.Entry;
using Core.IO;
using DZConfig;
using Param.Models;
using Param.Options;

public static class RVBankDirectoryExtensions
{

    public static IRVBankDataEntry? LocateConfigEntry(this IRVBankDirectory directory) =>
        directory.GetDataEntry("config.cpp") ?? directory.GetDataEntry("config.bin");

    public static IParamFile? LocateConfigFile(this IRVBankDirectory directory, ParamOptions paramOptions)
    {
        var entry = LocateConfigEntry(directory);
        if (entry is null)
        {
            return null;
        }

        using var reader = new BisBinaryReader(entry.EntryData, paramOptions.Charset, true);
        return new ParamFile("config", reader, paramOptions, directory.Logger);
    }

    public static DzConfig? LocateAddonConfig(this IRVBankDirectory directory, ParamOptions paramOptions)
    {
        var param = LocateConfigFile(directory, paramOptions);
        return param is null ? null : new DzConfig(param);
    }

    public static IEnumerable<IRVBankDataEntry> LocateConfigEntries(this IRVBankDirectory directory, SearchOption option)
    {
        var configs = new List<IRVBankDataEntry>();
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

    public static IEnumerable<IParamFile> LocateConfigFile(this IRVBankDirectory directory, ParamOptions paramOptions, SearchOption option) =>
        LocateConfigEntries(directory, option).Select(it =>
        {
            using var reader = new BisBinaryReader(it.EntryData, paramOptions.Charset, true);
            return new ParamFile("config", reader, paramOptions, directory.Logger);
        });

    public static IEnumerable<DzConfig> LocateAddonConfig(this IRVBankDirectory directory, ParamOptions paramOptions,
        SearchOption option) => LocateConfigFile(directory, paramOptions, option).Select(it => new DzConfig(it));
}
