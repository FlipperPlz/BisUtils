namespace BisUtils.Param;

using FResults;
using Options;
using Parse;
using PreProcessor.RV;
using PreProcessor.RV.Models.Directives;

public class test
{
    public static void Main()
    {
    }

    private static Result IncludeLocator(IRVIncludeDirective include, out string filetext)
    {
        Console.WriteLine($"found include {include.IncludeTarget}");
        filetext = "";
        return Result.Ok();
    }
}
