namespace BisUtils.Extensions.RVBank.DzConfigExtensions;

using BisUtils.RVBank.Extensions;
using BisUtils.RVBank.Model.Entry;
using Core.IO;
using DZConfig;
using Param.Models;
using Param.Options;

public static class RVBankDirectoryExtensions
{

    public static IRVBankDataEntry? LocateConfigEntry(this RVBankDirectory directory) =>
        directory.GetDataEntry("config.cpp") ?? directory.GetDataEntry("config.bin");

    public static IParamFile? LocateConfigFile(this RVBankDirectory directory, ParamOptions paramOptions)
    {
        var entry = LocateConfigEntry(directory);
        if (entry is null)
        {
            return null;
        }

        using var reader = new BisBinaryReader(entry.EntryData, paramOptions.Charset, true);
        return new ParamFile("config", reader, paramOptions, directory.Logger);
    }

    public static DzConfig? LocateAddonConfig(this RVBankDirectory directory, ParamOptions paramOptions)
    {
        var param = LocateConfigFile(directory, paramOptions);
        return param is null ? null : new DzConfig(param);
    }

}
