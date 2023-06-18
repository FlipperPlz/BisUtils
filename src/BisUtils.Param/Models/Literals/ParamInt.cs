namespace BisUtils.Param.Models.Literals;

using System.Text;
using Core.IO;
using FResults;
using Options;
using Stubs;

public interface IParamInt : IParamNumericLiteral<int>
{

}

public struct ParamInt : IParamInt
{
    public IParamFile? ParamFile { get; set; }
    public Result? LastResult { get; private set; }
    public required int ParamValue { get; set; }

    public Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.Write(ParamValue);
        return Result.ImmutableOk();
    }

    public Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        ParamValue = reader.ReadInt32();
        return Result.ImmutableOk();
    }

    public Result Validate(ParamOptions options) => Result.ImmutableOk();


    public Result WriteParam(StringBuilder builder, ParamOptions options)
    {
        builder.Append(ParamValue);
        return Result.ImmutableOk();
    }

}
