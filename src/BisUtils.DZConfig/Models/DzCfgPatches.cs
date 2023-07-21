namespace BisUtils.DZConfig.Models;

using Param.Enumerations;
using Param.Extensions;
using Param.Models.Literals;
using Param.Models.Statements;
using Param.Models.Stubs;
using Param.Utils;

public interface IDzCfgPatches : IParamConfigAbstraction<ParamClass>
{
    public string PatchName { get; set; }
    public IEnumerable<string>? Dependencies { get; set; }
}

public class DzCfgPatches : ParamConfigAbstraction<ParamClass>, IDzCfgPatches
{
    public DzCfgPatches(ParamClass ctx) : base(ctx)
    {
    }

    public string PatchName
    {
        get => ClassName;
        set => ClassName = value;
    }


    public IEnumerable<string>? Dependencies
    {
        get => GetArrayValues("requiredAddons");
        set => SetArrayValues("requiredAddons", value);
    }
}
