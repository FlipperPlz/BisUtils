namespace BisUtils.RvConfig.Models.Literals;

using System.Text;
using Core.Extensions;
using Core.IO;
using Enumerations;
using FResults;
using Microsoft.Extensions.Logging;
using Options;
using Stubs;
using Stubs.Holders;

public interface IParamString : IParamLiteral<string>
{
    ParamStringType StringType { get; }

    public bool ToInt(out ParamInt? paramInt);
    public bool ToFloat(out ParamFloat? paramFloat);
}

public class ParamString : ParamLiteral<string>, IParamString
{
    public override byte LiteralId => 4;
    public override string Value { get; set; } = null!;
    public ParamStringType StringType { get; }
    public static IParamString EmptyNoParents { get; } = new ParamString(null!, null!, null!, null!, null);

    public ParamString(string value, ParamStringType stringType, IRvConfigFile file, IParamLiteralHolder parent, ILogger? logger) : base(value, file, parent, logger) =>
        StringType = stringType;

    public ParamString(BisBinaryReader reader, ParamOptions options, IRvConfigFile file, IParamLiteralHolder parent, ILogger? logger) :
        base(reader, options, file, parent, logger)
    {
        StringType = ParamStringType.Quoted;
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        var result = base.Binarize(writer, options);
        writer.WriteAsciiZ(Value, options);
        return LastResult = result;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        LastResult = reader.ReadAsciiZ(out var paramValue, options);
        Value = paramValue;
        return LastResult;
    }

    public override Result Validate(ParamOptions options) =>
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
#pragma warning disable CA1305 //TODO: Options with locale

    public bool ToInt(out ParamInt? paramInt)
    {
        var response = int.TryParse(Value, out var parsedInt);
        if (!response)
        {
            paramInt = null;
            return response;
        }

        paramInt = new ParamInt(parsedInt, RvConfigFile, Parent, Logger);
        return true;
    }

    public bool ToFloat(out ParamFloat? paramFloat)
    {
        var response = float.TryParse(Value, out var parsedInt);
        if (!response)
        {
            paramFloat = null;
            return response;
        }
        paramFloat = new ParamFloat(parsedInt, RvConfigFile, Parent, null);
        return true;
    }

    public IParamFloat? ToFloat()
    {
        ToFloat(out var paramFloat);
        return paramFloat;
    }

    public IParamInt? ToInt()
    {
        ToInt(out var paramInt);
        return paramInt;
    }
#pragma warning restore CA1305

    public override Result WriteParam(ref StringBuilder builder, ParamOptions options)
    {
        LastResult = Stringify(out var value, Value, StringType, options);
        builder.Append(value);
        return LastResult;
    }
}
