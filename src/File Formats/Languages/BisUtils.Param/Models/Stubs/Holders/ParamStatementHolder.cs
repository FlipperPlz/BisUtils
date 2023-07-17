namespace BisUtils.Param.Models.Stubs.Holders;

using Core.IO;
using Extensions;
using FResults;
using Options;
using Statements;

public interface IParamStatementHolder : IParamElement
{
    IParamStatementHolder? ParentClass { get; }
    List<IParamStatement> Statements { get; }
    Result WriteStatements(out string value, ParamOptions options);

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


    public Result WriteStatements(out string value, ParamOptions options)
    {
        value = string.Join('\n', Statements.Select(s => s.ToParam(out _, options)));
        return Result.Ok();
    }

    public static IParamClass? operator >>(ParamStatementHolder clazz, string clazzName) =>
        clazz.LocateBaseClass(clazzName);
}
