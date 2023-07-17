namespace BisUtils.Param.Models.Stubs.Holders;

using System.Text;
using Core.IO;
using Extensions;
using FResults;
using Options;

public interface IParamLiteralHolder : IParamElement
{
    IParamLiteralHolder? ParentHolder { get; }
    List<IParamLiteral> Literals { get; }
    Result WriteLiterals(out string value, ParamOptions options);

}

public abstract class ParamLiteralHolder : ParamElement, IParamLiteralHolder
{
    public IParamLiteralHolder? ParentHolder { get; set; }
    public List<IParamLiteral> Literals { get; set; } = new();

    protected ParamLiteralHolder(IParamFile? file, IParamLiteralHolder? parent) : base(file) =>
        ParentHolder = parent;

    protected ParamLiteralHolder(IParamFile? file, IParamLiteralHolder? parent, BisBinaryReader reader, ParamOptions options) : base(file, reader, options) =>
        ParentHolder = parent;

    public Result WriteLiterals(out string value, ParamOptions options)
    {
        value = string.Join(',', Literals.Select(s => s.ToParam(out _, options)));
        return Result.Ok();
    }

}

