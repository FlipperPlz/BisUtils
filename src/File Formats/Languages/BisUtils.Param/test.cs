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
        var preprocessor = new RVPreProcessor(new List<IRVDefineDirective>(), IncludeLocator);
        ParamParser.Parse("""
            class CfgPatches {
                class MyMod {
                    arr[] = {1, 2, 3.4, Hiiiii, {another, "array,", 4444} };
                    variable2 = 3;
                    str = "woaaaah";
                    delete MyMod;
                };
            };


""",
            "config",
            preprocessor,
            out var file);
        file.ToParam(out var fileText, new ParamOptions());
        Console.WriteLine(fileText);
    }

    private static Result IncludeLocator(IRVIncludeDirective include, out string filetext)
    {
        Console.WriteLine($"found include {include.IncludeTarget}");
        filetext = "";
        return Result.Ok();
    }
}
