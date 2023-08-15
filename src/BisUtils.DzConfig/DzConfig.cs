﻿namespace BisUtils.DzConfig;

using Models;
using RvConfig.Extensions;
using RvConfig.Models;
using RvConfig.Utils;

public interface IDzConfig : IParamConfigAbstraction<IRvConfigFile>
{
    public IEnumerable<IDzCfgPatch>? CfgPatches { get; }
    public IEnumerable<IDzCfgMod>? CfgMods { get; }
}

public class DzConfig : ParamConfigAbstraction<IRvConfigFile>, IDzConfig
{
    public IEnumerable<IDzCfgPatch>? CfgPatches { get; private set; }

    public IEnumerable<IDzCfgMod>? CfgMods { get; private set; }

    public DzConfig(IRvConfigFile ctx) : base(ctx)
    {
        if (ParamContext.LocateBaseClass("CfgPatches") is { } patches)
        {
            CfgPatches = patches.LocateBaseClasses().Select(it => new DzCfgPatch(it));
        }

        CfgPatches = null;

        if (ParamContext.LocateBaseClass("CfgMods") is { } mods)
        {
            CfgMods = mods.LocateBaseClasses().Select(it => new DzCfgMod(it));
        }

        CfgMods = null;
    }

}
