namespace BisUtils.Param.Parse;

using BisUtils.Param.Models.Stubs;
using Core.Parsing;
using FResults;
using Lexer;
using Models;

public class ParamParser : IBisParser<ParamFile, ParamLexer, ParamTypes>
{

    public static Result Parse(out ParamFile? node, ParamLexer lexer)
    {
        var results = new List<Result>();
        node = new ParamFile("config", new List<IParamStatement>());
        var stack = new Stack<IParamStatementHolder>();
        stack.Push(node);
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

