namespace BisUtils.PreProcessor.RV;

using System.Text;
using Core.Parsing;
using FResults;

public class test
{
    public static void Main()
    {
        var preprocessor = new RVPreProcessor() {
            IncludeLocator = (path, builder) =>
            {
                Console.WriteLine(path);
                return Result.Ok();
            }
        };
        var lexer = new BisMutableStringStepper("""
                                                class    CfgPatches {
                                                    //My Comment :))

                                                    /**/
                                                    string="MyString #define"/**/;
                                                    please      d  a d  d y
                                                    #include <mememememememe>
                                                                              pappp\
                                                                              iiii
                                                }
                                                """);

        Console.WriteLine(lexer.ToString());
        var builder = new StringBuilder();
        preprocessor.OnTokenMatched += (match, bisLexer) => Console.WriteLine($"{match.TokenType.TokenId} = \"{match.TokenText}\"");
        preprocessor.EvaluateLexer(lexer, builder);
        Console.WriteLine(builder.ToString());
    }
}
