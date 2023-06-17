namespace BisUtils.Param.Models.Stubs;

using System.Text;
using Core.IO;
using FResults;
using Options;

public interface IParamStatementHolder : IParamElement
{
       List<IParamStatement> Statements { get; }

       Result WriteStatements(StringBuilder builder, ParamOptions options);

       StringBuilder WriteStatements(ParamOptions options)
       {
           var builder = new StringBuilder();
           WriteStatements(builder, options);
           return builder;
       }

       string StatementsText(ParamOptions options) =>
           WriteParam(options).ToString();
}

public abstract class ParamStatementHolder : ParamElement, IParamStatementHolder
{

    public List<IParamStatement> Statements { get; protected set; } = new();

    protected ParamStatementHolder(IParamFile? file) : base(file)
    {
    }

    protected ParamStatementHolder(BisBinaryReader reader, ParamOptions options) : base(reader, options)
    {
    }

    public override Result WriteParam(StringBuilder builder, ParamOptions options) =>
        WriteStatements(builder, options);

    public Result WriteStatements(StringBuilder builder, ParamOptions options) =>
        Result.Merge(Statements.Select(e => e.WriteParam(builder, options)));


    public StringBuilder WriteStatements(ParamOptions options)
    {
        var builder = new StringBuilder();
        WriteStatements(builder, options);
        return builder;
    }

    public string StatementsText(ParamOptions options) =>
        WriteParam(options).ToString();
}
