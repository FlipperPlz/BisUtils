namespace BisUtils.Param.Models.Stubs.Holders;

using System.Text;
using Core.IO;
using FResults;
using Options;

public interface IParamLiteralHolder : IParamElement
{
    IParamLiteralHolder? ParentHolder { get; }
    List<IParamLiteral> Literals { get; }
    StringBuilder WriteLiterals(out Result result, ParamOptions options);
    string WriteLiterals(ParamOptions options);
    Result TryWriteLiterals(StringBuilder builder, ParamOptions options);
    Result TryWriteLiterals(out string str, ParamOptions options);
}


public abstract class ParamLiteralHolder : ParamElement, IParamLiteralHolder
{
    public IParamLiteralHolder? ParentHolder { get; set; }
    public List<IParamLiteral> Literals { get; set; } = new();

    protected ParamLiteralHolder(IParamFile? file, IParamLiteralHolder? parent) : base(file) =>
        ParentHolder = parent;

    protected ParamLiteralHolder(IParamFile? file, IParamLiteralHolder? parent, BisBinaryReader reader, ParamOptions options) : base(file, reader, options) =>
        ParentHolder = parent;

    public StringBuilder WriteLiterals(out Result result, ParamOptions options)
    {
        var builder = new StringBuilder();
        result = WriteParam(builder, options);
        return builder;
    }

    public string WriteLiterals(ParamOptions options)
    {
        if (!TryWriteLiterals(out var str, options))
        {
            throw new Exception();
            //TODO result to exception
        };

        return str;
    }

    public Result TryWriteLiterals(StringBuilder builder, ParamOptions options) => throw new NotImplementedException();

    public Result TryWriteLiterals(out string str, ParamOptions options)
    {
        str = string.Join(',', Literals.Select(s => s.ToParam(options)));
        return Result.Ok();
    }
}

