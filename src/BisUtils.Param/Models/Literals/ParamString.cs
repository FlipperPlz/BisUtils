namespace BisUtils.Param.Models.Literals;

using System.Text;
using Core.IO;
using Enumerations;
using FResults;
using Options;
using Stubs;

public interface IParamString : IParamLiteral<string>
{
    ParamStringType StringType { get; }
    Result WriteStringifiedValue(StringBuilder builder, ParamOptions options);
}

public struct ParamString : IParamString
{
    public Result? LastResult { get; private set; } = null;
    public IParamFile? ParamFile { get; set; }
    public required ParamStringType StringType { get; set; } = ParamStringType.Unquoted;
    public required string ParamValue { get => paramValue; set => paramValue = value; }
    private string paramValue;

    public ParamString()
    {
    }

    public Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.WriteAsciiZ(ParamValue, options);
        return Result.ImmutableOk();
    }

    public Result Debinarize(BisBinaryReader reader, ParamOptions options) =>
        reader.ReadAsciiZ(out paramValue, options);

    public Result WriteParam(StringBuilder builder, ParamOptions options) =>
        WriteStringifiedValue(builder, options);


    public Result WriteStringifiedValue(StringBuilder builder, ParamOptions options) => StringType switch
    {
        ParamStringType.Quoted => throw new NotImplementedException(),
        ParamStringType.Unquoted => throw new NotImplementedException(),
        _ => Result.Fail($"Unknown string format {StringType}")
    };


    public Result Validate(ParamOptions options)
    {
        throw new NotImplementedException();
        return LastResult = Result.ImmutableOk();
    }

}
