namespace BisUtils.DZConfig.Models;

using Param.Models.Statements;
using Param.Utils;

public interface IDzMod : IParamConfigAbstraction<IParamClass>
{
    public IEnumerable<string>? Dependencies { get; set; }
    public DzPathCtx<ParamPathString>? EngineScriptModule { get; }
}

public class DzMod : ParamConfigAbstraction<IParamClass>, IDzMod
{
    public IEnumerable<string>? Dependencies
    {
        get => GetArrayValues("dependencies");
        set => SetArrayValues("dependencies", value);
    }

    public DzPathCtx<ParamPathString>? EngineScriptModule
    {
        get;
    }

    public DzMod(IParamClass ctx) : base(ctx)
    {

    }
}
