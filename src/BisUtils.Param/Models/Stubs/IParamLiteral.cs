namespace BisUtils.Param.Models.Stubs;

using System.Text;
using FResults;
using Options;

public interface IParamLiteralBase : IParamElement
{
    object? UnboxedParamValue { get; }
}

public interface IParamLiteral<out T> : IParamLiteralBase
{
    object? IParamLiteralBase.UnboxedParamValue => ParamValue;

    T? ParamValue { get; }

    Result IParamElement.WriteParam(StringBuilder builder, ParamOptions options)
    {
        var result = ToParam(out var str, options);
        builder.Append(str);
        return result;
    }

    StringBuilder IParamElement.WriteParam(out Result result, ParamOptions options)
    {
        var builder = new StringBuilder();
        result = WriteParam(builder, options);
        return builder;
    }

    string IParamElement.ToParam(ParamOptions options)
    {
        ToParam(out var str, options);
        return str;
    }

}
