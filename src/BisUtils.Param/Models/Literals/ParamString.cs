namespace BisUtils.Param.Models.Literals;

using Core.IO;
using Enumerations;
using FResults;
using Options;
using Stubs;

public interface IParamString : IParamLiteral<string>
{
    ParamStringType StringType { get; }
}

public struct ParamString : IParamString
{
    public Result? LastResult { get; private set; } = null;
    public IParamFile? ParamFile { get; set; }

    public required ParamStringType StringType { get; set; } = ParamStringType.Unquoted;
    public required string ParamValue { get => paramValue; set => paramValue = value; }
    private string paramValue = "";

    public ParamString()
    {
    }


    public Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        if (options.WriteLiteralId)
        {
            writer.Write(options.LiteralIdFoster(this));
        }

        writer.WriteAsciiZ(ParamValue, options);
        return LastResult = Result.ImmutableOk();
    }

    public Result Debinarize(BisBinaryReader reader, ParamOptions options) =>
        LastResult = reader.ReadAsciiZ(out paramValue, options);

    public Result ToParam(out string str, ParamOptions options) =>
        LastResult = Stringify(out str, ParamValue, StringType, options);

    public Result Validate(ParamOptions options) =>
        LastResult = Result.ImmutableOk();

    public static Result Stringify(out string stringified, string str, ParamStringType stringType, ParamOptions options)
    {
        switch (stringType)
        {
            case ParamStringType.Quoted:
            case ParamStringType.Unquoted:
                stringified = ""; //TODO:
                return Result.ImmutableOk();
            default:
                stringified = "";
                return Result.Fail("");
        }
    }




}
