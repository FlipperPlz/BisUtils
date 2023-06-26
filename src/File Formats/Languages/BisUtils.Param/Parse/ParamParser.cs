namespace BisUtils.Param.Parse;

using BisUtils.Param.Models.Stubs;
using Core.Parsing.Errors;
using Enumerations;
using FResults;
using Lexer;
using Models;
using Models.Literals;
using Models.Statements;
using PreProcessor.RV;

public static class ParamParser
{

    public static Result Parse(string contents, string filename, IRVPreProcessor preProcessor, out ParamFile file)
    {
        var results = new List<Result>();
        file = new ParamFile(filename, new List<IParamStatement>());
        var lexer = new ParamLexer(contents);
        var stack = new Stack<IParamStatementHolder>();
        stack.Push(file);
        //results.Add(preProcessor.ProcessLexer(lexer));
        lexer.ResetLexer();

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

