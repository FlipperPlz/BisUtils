namespace BisUtils.Param.Parse;

using System.Text;
using BisUtils.Param.Models.Stubs;
using FResults;
using Lexer;
using Models;
using PreProcessor.RV;

public static class ParamParser
{

    public static Result Parse(string contents, string filename, out ParamFile file, IRVPreProcessor? preProcessor = null)
    {
        var results = new List<Result>();
        file = new ParamFile(filename, new List<IParamStatement>());
        var lexer = new ParamLexer(contents);
        var stack = new Stack<IParamStatementHolder>();
        stack.Push(file);
        if (preProcessor is { } processor)
        {

            var builder = new StringBuilder();
            var result = processor.EvaluateLexer(lexer, builder);

        }

        while (stack.Any())
        {
            var context = stack.Peek();
        }
        Done:
        {

            return Result.Merge(results);
        }
    }
}

