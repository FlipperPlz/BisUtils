namespace BisUtils.Param.Models.Stubs;

using System.Text;
using Core.Family;
using Core.IO;
using FResults;
using Options;

public interface IParamStatementHolder : IParamElement, IFamilyParent
{
    IParamStatementHolder? ParentClass { get; }
    List<IParamStatement> Statements { get; }
    Result WriteStatements(StringBuilder builder, ParamOptions options);
    StringBuilder WriteStatements(out Result result, ParamOptions options);
    Result GetStatements(out string str, ParamOptions options);
    string GetStatements(ParamOptions options);


    IFamilyParent? IFamilyChild.Parent => ParentClass;
    IEnumerable<IFamilyMember> IFamilyParent.Children => Statements;
}

public abstract class ParamStatementHolder : ParamElement, IParamStatementHolder
{
    public IParamStatementHolder? ParentClass { get; set; }
    public List<IParamStatement> Statements { get; set; } = new();

    protected ParamStatementHolder(IParamFile? file, IParamStatementHolder? parent) : base(file) =>
        ParentClass = parent;

    protected ParamStatementHolder(IParamFile? file, BisBinaryReader reader, ParamOptions options) : base(file, reader, options)
    {
    }

    public override Result ToParam(out string str, ParamOptions options) =>
        GetStatements(out str, options);

    public Result GetStatements(out string str, ParamOptions options)
    {
        str = string.Join('\n', Statements.Select(s => s.ToParam(options)));
        return Result.Ok();
    }

    public string GetStatements(ParamOptions options)
    {
        GetStatements(out var str, options);
        return str;
    }


    public Result WriteStatements(StringBuilder builder, ParamOptions options)
    {
        var result = ToParam(out var str, options);
        builder.Append(str);
        return result;
    }

    public StringBuilder WriteStatements(out Result result, ParamOptions options)
    {
        var builder = new StringBuilder();
        result = WriteParam(builder, options);
        return builder;
    }

}
