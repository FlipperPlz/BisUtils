namespace BisUtils.Param.Models.Literals;

using System.Text;
using Core.IO;
using FResults;
using Options;
using Stubs;

public interface IParamFloat : IParamNumericLiteral<float>
{

}

public struct ParamFloat : IParamFloat
{
    public Result? LastResult { get; private set; }
    public IParamFile? ParamFile { get; set; }
    public required float ParamValue { get; set; }

    public Result Binarize(BisBinaryWriter writer, ParamOptions options) => throw new NotImplementedException();

    public Result Validate(ParamOptions options) => throw new NotImplementedException();

    public Result Debinarize(BisBinaryReader reader, ParamOptions options) => throw new NotImplementedException();

    public Result WriteParam(StringBuilder builder, ParamOptions options) => throw new NotImplementedException();

}
