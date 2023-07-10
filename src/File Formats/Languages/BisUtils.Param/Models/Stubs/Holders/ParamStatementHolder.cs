namespace BisUtils.Param.Models.Stubs.Holders;

using System.Text;
using Core.Extensions;
using Core.IO;
using Enumerations;
using FResults;
using Literals;
using Options;
using Statements;

public interface IParamStatementHolder : IParamElement
{
    IParamStatementHolder? ParentClass { get; }
    List<IParamStatement> Statements { get; }
    StringBuilder WriteStatements(out Result result, ParamOptions options);
    string WriteStatements(ParamOptions options);
    Result TryWriteStatements(StringBuilder builder, ParamOptions options);
    Result TryWriteStatements(out string str, ParamOptions options);

    public IEnumerable<T> GetStatements<T>() =>
        Statements.OfType<T>();

    public IEnumerable<IParamExternalClass> LocateAllClasses() =>
        GetStatements<IParamExternalClass>();

    public IEnumerable<IParamExternalClass> LocateAllClasses(string classname) =>
        LocateAllClasses().Where(@class => @class.ClassName == classname);

    public IParamExternalClass? LocateAnyClass(string classname) =>
        LocateAllClasses(classname).FirstOrDefault();

    public IEnumerable<IParamClass> LocateBaseClasses() =>
        GetStatements<IParamClass>();

    public IEnumerable<IParamClass> LocateBaseClasses(string classname) =>
        LocateBaseClasses().Where(@class => @class.ClassName == classname);

    public IParamClass? LocateBaseClass(string classname) =>
        LocateBaseClasses(classname).FirstOrDefault();

    public IEnumerable<ParamExternalClass> LocateExternalClasses() =>
        GetStatements<ParamExternalClass>();

    public IEnumerable<ParamExternalClass> LocateExternalClasses(string classname) =>
        LocateExternalClasses().Where(@class => @class.ClassName == classname);

    public ParamExternalClass? LocateExternalClass(string classname) =>
        LocateExternalClasses(classname).FirstOrDefault();

    public IEnumerable<IParamDelete> LocateDeleteStatements() =>
        GetStatements<IParamDelete>();

    public IEnumerable<IParamDelete> LocateDeleteStatements(string target) =>
        LocateDeleteStatements().Where(e => e.DeleteTargetName == target);

    public IParamDelete? LocateDeleteStatement(string target) =>
        LocateDeleteStatements(target).FirstOrDefault();

    public IEnumerable<IParamVariable> LocateAllVariables() => GetStatements<IParamVariable>();

    public IEnumerable<IParamVariable> LocateVariables(string name) =>
        GetStatements<IParamVariable>().Where(e => e.VariableName == name);

    public IEnumerable<IParamVariable> LocateVariables<T>(string name) where T : IParamLiteral =>
        GetStatements<IParamVariable>().Where(e => e.VariableName == name && e.VariableValue is T?);

    public IParamVariable? LocateVariable(string name) =>
        LocateVariables(name).FirstOrDefault();

    public IParamArray? LocateArray(string name, out ParamOperatorType? op)
    {
        var var = LocateVariables<IParamArray>(name).FirstOrDefault();
        op = var?.VariableOperator;
        return (IParamArray?) var?.VariableValue;
    }

    public IParamVariable? LocateVariable<T>(string name) where T : IParamLiteral =>
        LocateVariables<T>(name).FirstOrDefault();

    public T? EvaluateVariable<T>(string name) where T : IParamLiteral =>
        (T?) LocateVariable<T>(name)?.VariableValue;

    public bool HasVariable(string name, out IParamVariable? variable) =>
        (variable = LocateVariable(name)) is not null;

    public bool HasVariable<T>(string name, out IParamVariable? variable) where T : IParamLiteral =>
        (variable = LocateVariable<T>(name)) is not null;

    public bool HasExternalClass(string name, out ParamExternalClass? clazz) =>
        (clazz = LocateExternalClass(name)) is not null;

    public bool HasBaseClass(string name, out IParamClass? clazz) =>
        (clazz = LocateBaseClass(name)) is not null;

    public bool HasClass(string name, out IParamExternalClass? clazz) =>
        (clazz = LocateAnyClass(name)) is not null;

    public bool WasDeleted(string target, out IParamDelete? deleteStatement) =>
        (deleteStatement = LocateDeleteStatement(target)) is not null;

    public static IParamClass? operator >>(IParamStatementHolder clazz, string clazzName) =>
        clazz.LocateBaseClass(clazzName);
}

public abstract class ParamStatementHolder : ParamElement, IParamStatementHolder
{
    public IParamStatementHolder? ParentClass { get; set; }
    public List<IParamStatement> Statements { get; protected init; } = new();

    protected ParamStatementHolder
    (
        IParamFile? file,
        IParamStatementHolder? parent,
        IEnumerable<IParamStatement>? statements
    ) : base(file)
    {
        ParentClass = parent;
        Statements = statements?.ToList() ?? new List<IParamStatement>();
    }

    protected ParamStatementHolder(IParamFile? file, IParamStatementHolder? parent, BisBinaryReader reader, ParamOptions options) : base(file, reader, options) =>
        ParentClass = parent;

    public override Result ToParam(out string str, ParamOptions options) =>
        TryWriteStatements(out str, options);


    public StringBuilder WriteStatements(out Result result, ParamOptions options)
    {
        var builder = new StringBuilder();
        result = WriteParam(builder, options);
        return builder;
    }

    public string WriteStatements(ParamOptions options)
    {
        if (!(LastResult = TryWriteStatements(out var str, options)))
        {
            LastResult.Throw();
        }

        return str;
    }

    public Result TryWriteStatements(StringBuilder builder, ParamOptions options)
    {
        var result = TryWriteStatements(out var str, options);
        builder.Append(str);
        return result;
    }

    public Result TryWriteStatements(out string str, ParamOptions options)
    {
        str = string.Join('\n', Statements.Select(s => s.ToParam(options)));
        return Result.Ok();
    }

    public IEnumerable<T> GetStatements<T>() =>
        Statements.OfType<T>();

    public IEnumerable<IParamExternalClass> LocateAllClasses() =>
        GetStatements<IParamExternalClass>();

    public IEnumerable<IParamExternalClass> LocateAllClasses(string classname) =>
        LocateAllClasses().Where(@class => @class.ClassName == classname);

    public IParamExternalClass? LocateAnyClass(string classname) =>
        LocateAllClasses(classname).FirstOrDefault();

    public IEnumerable<IParamClass> LocateBaseClasses() =>
        GetStatements<IParamClass>();

    public IEnumerable<IParamClass> LocateBaseClasses(string classname) =>
        LocateBaseClasses().Where(@class => @class.ClassName == classname);

    public IParamClass? LocateBaseClass(string classname) =>
        LocateBaseClasses(classname).FirstOrDefault();

    public IEnumerable<ParamExternalClass> LocateExternalClasses() =>
        GetStatements<ParamExternalClass>();

    public IEnumerable<ParamExternalClass> LocateExternalClasses(string classname) =>
        LocateExternalClasses().Where(@class => @class.ClassName == classname);

    public ParamExternalClass? LocateExternalClass(string classname) =>
        LocateExternalClasses(classname).FirstOrDefault();

    public IEnumerable<IParamDelete> LocateDeleteStatements() =>
        GetStatements<IParamDelete>();

    public IEnumerable<IParamDelete> LocateDeleteStatements(string target) =>
        LocateDeleteStatements().Where(e => e.DeleteTargetName == target);

    public IParamDelete? LocateDeleteStatement(string target) =>
        LocateDeleteStatements(target).FirstOrDefault();

    public IEnumerable<IParamVariable> LocateAllVariables() => GetStatements<IParamVariable>();

    public IEnumerable<IParamVariable> LocateVariables(string name) =>
        GetStatements<IParamVariable>().Where(e => e.VariableName == name);

    public IEnumerable<IParamVariable> LocateVariables<T>(string name) where T : IParamLiteral =>
        GetStatements<IParamVariable>().Where(e => e.VariableName == name && e.VariableValue is T?);

    public IParamVariable? LocateVariable(string name) =>
        LocateVariables(name).FirstOrDefault();

    public IParamArray? LocateArray(string name, out ParamOperatorType? op)
    {
        var var = LocateVariables<IParamArray>(name).FirstOrDefault();
        op = var?.VariableOperator;
        return (IParamArray?) var?.VariableValue;
    }

    public IParamVariable? LocateVariable<T>(string name) where T : IParamLiteral =>
        LocateVariables<T>(name).FirstOrDefault();

    public T? EvaluateVariable<T>(string name) where T : IParamLiteral =>
        (T?) LocateVariable<T>(name)?.VariableValue;

    public bool HasVariable(string name, out IParamVariable? variable) =>
        (variable = LocateVariable(name)) is not null;

    public bool HasVariable<T>(string name, out IParamVariable? variable) where T : IParamLiteral =>
        (variable = LocateVariable<T>(name)) is not null;

    public bool HasExternalClass(string name, out ParamExternalClass? clazz) =>
        (clazz = LocateExternalClass(name)) is not null;

    public bool HasBaseClass(string name, out IParamClass? clazz) =>
        (clazz = LocateBaseClass(name)) is not null;

    public bool HasClass(string name, out IParamExternalClass? clazz) =>
        (clazz = LocateAnyClass(name)) is not null;

    public bool WasDeleted(string target, out IParamDelete? deleteStatement) =>
        (deleteStatement = LocateDeleteStatement(target)) is not null;

    public static IParamClass? operator >>(ParamStatementHolder clazz, string clazzName) =>
        clazz.LocateBaseClass(clazzName);
}
