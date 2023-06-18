namespace BisUtils.Param.Models.Literals;

using System.Text;
using Core.IO;
using FResults;
using Options;
using Stubs;

public interface IParamArray : IParamLiteral<List<IParamLiteralBase>>
{

}
// ParamArray is reserved? why...
#pragma warning disable CA1716
public struct ParamArray : IParamArray
{
    public IParamFile? ParamFile { get; set; }
    public Result? LastResult { get; private set; }
    public required List<IParamLiteralBase> ParamValue { get; set; }

    public Result Binarize(BisBinaryWriter writer, ParamOptions options) => throw new NotImplementedException();

    public Result Debinarize(BisBinaryReader reader, ParamOptions options) => throw new NotImplementedException();

    public Result Validate(ParamOptions options) =>
        LastResult = Result.Merge(ParamValue.Select(v => v.Validate(options)));

    public Result ToParam(out string str, ParamOptions options)
    {
        str = $"{{{string.Join(", ", ParamValue.Select(v => v.ToParam(options)))}}}";
        return Result.Merge
        (
            ParamValue.Where(v => v.LastResult is not null)
                .Select(v => v.LastResult!)
        );
    }
}
