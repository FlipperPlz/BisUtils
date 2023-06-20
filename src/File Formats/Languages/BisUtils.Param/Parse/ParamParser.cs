namespace BisUtils.Param.Parse;

using BisUtils.Param.Models.Stubs;
using Core.Parsing.Errors;
using FResults;
using Lexer;
using Models;
using Models.Statements;
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

            var skipped = -1;
            switch (keyword)
            { //TODO: Parse Statements
                case "delete":
                {
                    results.Add(lexer.TraverseWhitespace(out skipped));
                    results.Add(Result.FailIf(skipped <= 1, "Expected whitespace."));
                    results.Add(lexer.ReadIdentifier(out keyword, false));
                    context.Statements.Add(new ParamDelete(file, context, keyword));
                    continue;
                }
                case "class":
                {
                    results.Add(lexer.TraverseWhitespace(out skipped));
                    results.Add(Result.FailIf(skipped <= 1, "Expected whitespace."));
                    results.Add(lexer.ReadIdentifier(out keyword, false));
                    results.Add(lexer.TraverseWhitespace(out skipped));
                    string? baseClass = null;
                    switch (lexer.CurrentChar)
                    {
                        case ';':
                            context.Statements.Add(new ParamExternalClass(file, context, keyword));
                            continue;
                        case ':':
                        {
                            lexer.MoveForward();
                            results.Add(lexer.TraverseWhitespace(out _));
                            results.Add(lexer.ReadIdentifier(out baseClass));
                            results.Add(lexer.TraverseWhitespace(out _));
                            break;
                        }
                        case '{':
                        {
                            break;
                        }
                        default:
                        {
                            results.Add(Result.Fail("Unexpected char"));
                            return Result.Merge(results);
                        }
                    }

                    if (lexer.CurrentChar != '{')
                    {
                        results.Add(Result.Fail("Unexpected char"));
                    }

                    var clazz = new ParamClass(file, context, keyword, baseClass, new List<IParamStatement>());
                    context.Statements.Add(clazz);
                    stack.Push(clazz);
                    continue;
                }
                case "enum":
                    //TODO
                default:
                    results.Add(lexer.TraverseWhitespace(out skipped));

                    break;
            }
        }
        Done:
        {

            return Result.Merge(results);
        }
    }
}

