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

    public Result Validate(ParamOptions options) => throw new NotImplementedException();

    public Result Debinarize(BisBinaryReader reader, ParamOptions options) => throw new NotImplementedException();

    public Result WriteParam(StringBuilder builder, ParamOptions options) => throw new NotImplementedException();

}
