namespace BisUtils.Param.Extensions;

using FResults;
using Models.Stubs.Holders;
using Options;

public static class ParamLiteralHolderExtensions
{
    public static string WriteLiterals(this IParamLiteralHolder element, out Result result, ParamOptions options)
    {
        result = element.WriteLiterals(out var val, options);
        return val;
    }
}
