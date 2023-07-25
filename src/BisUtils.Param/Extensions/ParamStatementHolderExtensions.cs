namespace BisUtils.Param.Extensions;

using Enumerations;
using Microsoft.Extensions.Logging;
using Models.Literals;
using Models.Statements;
using Models.Stubs;
using Models.Stubs.Holders;

public static class ParamStatementHolderExtensions
{
    public static IEnumerable<T> GetStatements<T>(this IParamStatementHolder holder) =>
        holder.Statements.OfType<T>();

    public static IEnumerable<IParamExternalClass> LocateAllClasses(this IParamStatementHolder holder) =>
        GetStatements<IParamExternalClass>(holder);

    public static IEnumerable<IParamExternalClass> LocateAllClasses(this IParamStatementHolder holder, string classname) =>
        LocateAllClasses(holder).Where(@class => @class.ClassName == classname);

    public static IParamExternalClass? LocateAnyClass(this IParamStatementHolder holder, string classname) =>
        LocateAllClasses(holder, classname).FirstOrDefault();

    public static IEnumerable<IParamClass> LocateBaseClasses(this IParamStatementHolder holder) =>
        GetStatements<IParamClass>(holder);

    public static IEnumerable<IParamClass> LocateBaseClasses(this IParamStatementHolder holder, string classname) =>
        LocateBaseClasses(holder).Where(@class => @class.ClassName == classname);

    public static IParamClass? LocateBaseClass(this IParamStatementHolder holder, string classname) =>
        LocateBaseClasses(holder, classname).FirstOrDefault();

    public static IEnumerable<ParamExternalClass> LocateExternalClasses(this IParamStatementHolder holder) =>
        GetStatements<ParamExternalClass>(holder);

    public static IEnumerable<ParamExternalClass> LocateExternalClasses(this IParamStatementHolder holder, string classname) =>
        LocateExternalClasses(holder).Where(@class => @class.ClassName == classname);

    public static ParamExternalClass? LocateExternalClass(this IParamStatementHolder holder, string classname) =>
        LocateExternalClasses(holder, classname).FirstOrDefault();

    public static IEnumerable<IParamDelete> LocateDeleteStatements(this IParamStatementHolder holder) =>
        GetStatements<IParamDelete>(holder);

    public static IEnumerable<IParamDelete> LocateDeleteStatements(this IParamStatementHolder holder, string target) =>
        LocateDeleteStatements(holder).Where(e => e.DeleteTargetName == target);

    public static IParamDelete? LocateDeleteStatement(this IParamStatementHolder holder, string target) =>
        LocateDeleteStatements(holder, target).FirstOrDefault();

    public static IEnumerable<IParamVariable> LocateAllVariables(this IParamStatementHolder holder) =>
        GetStatements<IParamVariable>(holder);

    public static IEnumerable<IParamVariable> LocateVariables(this IParamStatementHolder holder, string name) =>
        GetStatements<IParamVariable>(holder).Where(e => e.VariableName == name);

    public static IEnumerable<IParamVariable> LocateVariables<T>(this IParamStatementHolder holder, string name) where T : IParamLiteral =>
        GetStatements<IParamVariable>(holder).Where(e => e.VariableName == name && e.VariableValue is T?);

    public static IParamVariable? LocateVariable(this IParamStatementHolder holder, string name) =>
        LocateVariables(holder, name).FirstOrDefault();

    public static IParamArray? LocateArray(this IParamStatementHolder holder, string name, out ParamOperatorType? op)
    {
        var var = LocateVariables<IParamArray>(holder, name).FirstOrDefault();
        op = var?.VariableOperator;
        return (IParamArray?) var?.VariableValue;
    }

    public static IParamVariable? LocateVariable<T>(this IParamStatementHolder holder, string name) where T : IParamLiteral =>
        LocateVariables<T>(holder, name).FirstOrDefault();

    public static IEnumerable<T>? GetArrayValues<T>(this IParamStatementHolder context, string variableName) where T : IParamLiteral
    {
        if (context.LocateVariable<IParamArray>(variableName) is { } variable)
        {
            return ((ParamArray)variable.VariableValue).Value.OfType<T>();
        }

        return null;
    }

    public static T CreateVariable<T>(this IParamStatementHolder ctx, string name, T value, ILogger? logger) where T : IParamLiteral => (T) new ParamVariable(name, value, ParamOperatorType.Assign, ctx.ParamFile, ctx, logger).VariableValue;

    public static T AddVariable<T>(this IParamStatementHolder ctx, string name, T value, ILogger? logger) where T : IParamLiteral
    {
        var variable = new ParamVariable(name, value, ParamOperatorType.Assign, ctx.ParamFile, ctx, logger);
        ctx.Statements.Add(variable);
        return (T)variable.VariableValue;

    }

    public static void RemoveStatement(this IParamStatementHolder holder, IParamStatement statement) => holder.Statements.Remove(statement);

    public static void RemoveStatements(this IParamStatementHolder holder, IEnumerable<IParamStatement> statements)
    {
        foreach (var statement in statements)
        {
            holder.RemoveStatement(statement);
        }
    }

    public static T? EvaluateVariable<T>(this IParamStatementHolder holder, string name) where T : IParamLiteral =>
        (T?) LocateVariable<T>(holder, name)?.VariableValue;

    public static bool HasVariable(this IParamStatementHolder holder, string name, out IParamVariable? variable) =>
        (variable = LocateVariable(holder, name)) is not null;

    public static bool HasVariable<T>(this IParamStatementHolder holder, string name, out IParamVariable? variable) where T : IParamLiteral =>
        (variable = LocateVariable<T>(holder, name)) is not null;

    public static bool HasExternalClass(this IParamStatementHolder holder, string name, out ParamExternalClass? clazz) =>
        (clazz = LocateExternalClass(holder, name)) is not null;

    public static bool HasBaseClass(this IParamStatementHolder holder, string name, out IParamClass? clazz) =>
        (clazz = LocateBaseClass(holder, name)) is not null;

    public static bool HasClass(this IParamStatementHolder holder, string name, out IParamExternalClass? clazz) =>
        (clazz = LocateAnyClass(holder, name)) is not null;

    public static bool WasDeleted(this IParamStatementHolder holder, string target, out IParamDelete? deleteStatement) =>
        (deleteStatement = LocateDeleteStatement(holder, target)) is not null;

}
