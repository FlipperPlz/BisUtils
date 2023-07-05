namespace BisUtils.Param.Models.Stubs.Holders;

using System.Text;
using BisUtils.Core.IO;
using BisUtils.Param.Options;
using FResults;

public interface IParamStatementHolder : IParamElement
{
    IParamStatementHolder? ParentClass { get; }
    List<IParamStatement> Statements { get; }
    StringBuilder WriteStatements(out Result result, ParamOptions options);
    string WriteStatements(ParamOptions options);
    Result TryWriteStatements(StringBuilder builder, ParamOptions options);
    Result TryWriteStatements(out string str, ParamOptions options);

}

public abstract class ParamStatementHolder : ParamElement, IParamStatementHolder
{
    public IParamStatementHolder? ParentClass { get; set; }
    public List<IParamStatement> Statements { get; set; } = new();

    protected ParamStatementHolder(IParamFile? file, IParamStatementHolder? parent,
        IEnumerable<IParamStatement>? statements) : base(file)
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
        if (!TryWriteStatements(out var str, options))
        {
            throw new Exception();
            //TODO result to exception
        };

        return str;
    }

    public Result TryWriteStatements(StringBuilder builder, ParamOptions options) => throw new NotImplementedException();

    public Result TryWriteStatements(out string str, ParamOptions options)
    {
        str = string.Join('\n', Statements.Select(s => s.ToParam(options)));
        return Result.Ok();
    }
}
