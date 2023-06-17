namespace BisUtils.Param.Models.Stubs;

using System.Text;
using Options;

public interface IParamLiteralBase : IParamElement
{
    object? UnboxedParamValue { get; }
}

public interface IParamLiteral<out T> : IParamLiteralBase
{
    object? IParamLiteralBase.UnboxedParamValue => ParamValue;

    T? ParamValue { get; }

    StringBuilder IParamElement.WriteParam(ParamOptions options)
    {
        var builder = new StringBuilder();
        WriteParam(builder, options);
        return builder;
    }

    string IParamElement.ToParam(ParamOptions options) =>
        WriteParam(options).ToString();
}
