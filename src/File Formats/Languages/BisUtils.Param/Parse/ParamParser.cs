namespace BisUtils.Param.Parse;

using BisUtils.Param.Models.Stubs;
using Core.Parsing.Errors;
using FResults;
using Lexer;
using Models;
using PreProcessor.RV;

public static class ParamLiteralParser
{

    public static Result Parse(string contents, string filename, IRVPreProcessor preProcessor, out ParamFile file)
    {
        var results = new List<Result>();
        file = new ParamFile(filename, new List<IParamStatement>());
        var lexer = new ParamLexer(contents);
        var stack = new Stack<IParamStatementHolder>();
        stack.Push(file);
        results.Add(preProcessor.ProcessLexer(lexer));
        lexer.ResetLexer();

        bool TryEnd()
        {
            if (stack is not { Count: 1 })
            {
                throw new IOException();
            }

            return true;
        }

        while (stack.Any())
        {
            var context = stack.Peek();
            lexer.MoveForward();
            results.Add(lexer.TraverseWhitespace(out _));

            switch (lexer.CurrentChar)
            {
                case null:
                {
                    if (TryEnd())
                    {
                        goto Done;
                    }

                    break;
                }
                case '#': return Result.Fail(BisEndOfFileError.Instance); //TODO
                case '}':
                {
                    lexer.MoveForward();
                    lexer.TraverseWhitespace(out _);
                    if(lexer.CurrentChar != ';')
                    {
                        return Result.Fail(BisEndOfFileError.Instance); //TODO
                    }

                    stack.Pop();
                    continue;
                }
            }

            results.Add(lexer.ReadIdentifier(out var keyword, true));

            switch (keyword)
            { //TODO: Parse Statements
                default:
                    break;
            }

        }
        Done:
        {

            return Result.Merge(results);
        }
    }
}

