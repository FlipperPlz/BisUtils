namespace BisUtils.Param.Extensions;

using FResults;
using Models.Stubs;
using Options;

public static class ParamElementExtensions
{
    public static string ToParam(this IParamElement element, out Result result, ParamOptions options)
    {
        result = element.WriteParam(out var val, options);
        return val;
    }
}
