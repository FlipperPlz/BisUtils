namespace BisUtils.DzConfig.Models;

using RvConfig.Extensions;
using RvConfig.Models.Literals;
using RvConfig.Models.Statements;
using RvConfig.Utils;

public interface IDzCfgMod : IParamConfigAbstraction<IParamClass>
{
    public IEnumerable<string>? Dependencies { get; set; }
    public ParamXmlPath? InputsXmlPath { get; }
    public DzPathCtx<ParamPathString>? EngineScriptModule { get; }
    public DzPathCtx<ParamPathString>? GameLibScriptModule { get; }
    public DzPathCtx<ParamPathString>? GameScriptModule { get; }
    public DzPathCtx<ParamPathString>? WorldScriptModule { get; }
    public DzPathCtx<ParamPathString>? MissionScriptModule { get; }

}

public class DzCfgMod : ParamConfigAbstraction<IParamClass>, IDzCfgMod
{
    private IParamClass? DefinitionsClass { get; }
    //TODO: Logger on ParamConfigAbstraction
    public IEnumerable<string>? Dependencies
    {
        get => GetArrayValues("dependencies");
        set => SetArrayValues("dependencies", value);
    }


    public ParamXmlPath? InputsXmlPath { get; }

    public DzPathCtx<ParamPathString>? EngineScriptModule => LocateDefinition("engineScriptModule");
    public DzPathCtx<ParamPathString>? GameLibScriptModule => LocateDefinition("gameLibScriptModule");
    public DzPathCtx<ParamPathString>? GameScriptModule => LocateDefinition("gameScriptModule");
    public DzPathCtx<ParamPathString>? WorldScriptModule => LocateDefinition("worldScriptModule");
    public DzPathCtx<ParamPathString>? MissionScriptModule => LocateDefinition("missionScriptModule");



    private DzPathCtx<ParamPathString>? LocateDefinition(string name)
    {
        if (DefinitionsClass?.LocateBaseClass(name) is { } module)
        {
            return new DzPathCtx<ParamPathString>(module);
        }

        return null;
    }

    public DzCfgMod(IParamClass ctx) : base(ctx)
    {
        DefinitionsClass = ParamContext.LocateBaseClass("defs");
        if (ParamContext.LocateVariable<IParamString>("inputs") is { } inputsProp)
        {
            InputsXmlPath = new ParamXmlPath((IParamString)inputsProp.VariableValue);
        }

        InputsXmlPath = null;
    }
}
