namespace BisUtils.DzConfig.Models;

using RvConfig.Models.Statements;
using RvConfig.Utils;

public interface IDzCfgPatch : IParamConfigAbstraction<IParamClass>
{
    public string PatchName { get; set; }
    public IEnumerable<string>? Dependencies { get; set; }
}

public class DzCfgPatch : ParamConfigAbstraction<IParamClass>, IDzCfgPatch
{
    public DzCfgPatch(IParamClass ctx) : base(ctx)
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
