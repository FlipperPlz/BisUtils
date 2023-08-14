namespace BisUtils.RvConfig.Utils;

using Core.Logging;
using Enumerations;
using Extensions;
using Microsoft.Extensions.Logging;
using Models;
using Models.Literals;
using Models.Statements;
using Models.Stubs;

public interface IParamConfigAbstraction<TCtx> where TCtx : IParamClass
{
    public TCtx ParamContext { get; set; }
    public string ClassName { get; set; }
    public ILogger? Logger { get; }
}


//TODO(DRY): Refactor
public abstract class ParamConfigAbstraction<TCtx>: IParamConfigAbstraction<TCtx> where TCtx : class, IParamClass
{
    public TCtx ParamContext { get; set; }

    public string ClassName
    {
        get => ParamContext.ClassName;
        set => ParamContext.ClassName = value;
    }

    public ILogger? Logger { get; }

    protected ParamConfigAbstraction(TCtx ctx)
    {
        ParamContext = ctx;
        Logger = ((IBisLoggable)ParamContext).Logger;
    }

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
            var added = ParamContext.AddVariable<IParamString>(variableName, ParamString.EmptyNoParents, Logger);
            variable = (IParamVariable) added.Parent;
        }

        variable.VariableValue = (IParamLiteral) new ParamString(value, ParamStringType.Quoted, ParamContext.ParamFile, variable, Logger);

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
            var added = ParamContext.AddVariable<IParamArray>(variableName, ParamArray.Nill, Logger);
            variable = (IParamVariable) added.Parent;
        }

        variable.VariableValue = new ParamArray( values.Select(it =>
            (IParamLiteral)new ParamString(it, ParamStringType.Quoted, ParamContext.ParamFile, variable, Logger)), ParamContext.ParamFile, variable, Logger);
    }
}
