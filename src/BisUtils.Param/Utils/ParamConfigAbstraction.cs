namespace BisUtils.Param.Utils;

using Enumerations;
using Extensions;
using Models;
using Models.Literals;
using Models.Statements;
using Models.Stubs;

public interface IParamConfigAbstraction<TCtx> where TCtx : IParamClass
{
    public TCtx ParamContext { get; set; }
    public string ClassName { get; set; }
}


//TODO(DRY): Refactor
public abstract class ParamConfigAbstraction<TCtx>: IParamConfigAbstraction<TCtx> where TCtx : IParamClass
{
    public TCtx ParamContext { get; set; }

    public string ClassName
    {
        get => ParamContext.ClassName;
        set => ParamContext.ClassName = value;
    }

    public ParamConfigAbstraction(TCtx ctx) => ParamContext = ctx;

    protected IEnumerable<string>? GetArrayValues( string variableName)
    {
        if (ParamContext.LocateVariable<IParamArray>(variableName) is { } variable)
        {
            return ((ParamArray)variable.VariableValue).Value.OfType<ParamString>().Select(it => it.Value);
        }

        return null;
    }

    protected IEnumerable<IParamString>? GetActualArrayValues( string variableName)
    {
        if (ParamContext.LocateVariable<IParamArray>(variableName) is { } variable)
        {
            return ((ParamArray)variable.VariableValue).Value.OfType<ParamString>();
        }

        return null;
    }

    protected string? GetString(string variableName)
    {
        if (ParamContext.LocateVariable<IParamString>(variableName) is { } variable)
        {
            return ((ParamString)variable.VariableValue).Value;
        }

        return null;
    }

    protected void SetString(string variableName, string? value)
    {
        if (value is null)
        {
            ParamContext.RemoveStatements(ParamContext.LocateVariables<IParamString>(variableName));
            return;
        }
        if (ParamContext.LocateVariable<IParamString>(variableName) is not { } variable)
        {
            var added = ParamContext.AddVariable<IParamString>(variableName, ParamString.Nill);
            variable = (IParamVariable) added.Parent;
        }

        variable.VariableValue = (IParamLiteral) new ParamString(ParamContext.ParamFile, variable, value, ParamStringType.Quoted);

    }

    protected void SetArrayValues(string variableName, IEnumerable<string>? values)
    {
        if (values is null)
        {
            ParamContext.RemoveStatements(ParamContext.LocateVariables<IParamArray>(variableName));
            return;
        }
        if (ParamContext.LocateVariable<IParamArray>(variableName) is not { } variable)
        {
            var added = ParamContext.AddVariable<IParamArray>(variableName, ParamArray.Nill);
            variable = (IParamVariable) added.Parent;
        }

        variable.VariableValue = new ParamArray(ParamContext.ParamFile, variable, values.Select(it =>
            (IParamLiteral)new ParamString(ParamContext.ParamFile, variable, it, ParamStringType.Quoted)));
    }
}
