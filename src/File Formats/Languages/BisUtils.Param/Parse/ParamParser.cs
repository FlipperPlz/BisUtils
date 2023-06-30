namespace BisUtils.Param.Parse;

using BisUtils.Param.Models.Stubs;
using Core.Parsing;
using FResults;
using Lexer;
using Models;
using Models.Statements;
using PreProcessor.RV;

public class ParamParser : IBisParser<ParamFile, ParamLexer, ParamTypes, RVPreProcessor>
{
    public static readonly ParamParser Instance = new();

#pragma warning disable CA1822
    public Result Parse(out ParamFile? node, ParamLexer lexer)
    {
        var results = new List<Result>();
        node = new ParamFile("config", new List<IParamStatement>());
        var stack = new Stack<IParamStatementHolder>();
        stack.Push(node);
        var context = stack.Peek();
        while (stack.Any())
        {
            var next = lexer.NextToken();
            switch ((ParamTypes) next)
            {
                case ParamTypes.KwClass:
                {
                    next = lexer.NextToken();
                    if (next != ParamTypes.AbsWhitespace)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }

                    next = lexer.NextToken();
                    if (next != ParamTypes.AbsIdentifier)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    var classname = next.TokenText;
                    next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace);
                    switch ((ParamTypes) next)
                    {
                        case ParamTypes.SymSeparator:
                        {
                            context.Statements.Add(new ParamExternalClass(node, context, classname));
                            continue;
                        }
                        case ParamTypes.SymColon or ParamTypes.SymLCurly:
                            break;
                        default:
                            throw new NotSupportedException(); //TODO: Error
                    }

                    var superName = "";
                    if (next == ParamTypes.SymColon)
                    {
                        next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace);
                        if (next != ParamTypes.AbsIdentifier)
                        {
                            throw new NotSupportedException(); //TODO: Error
                        }

                        superName = next.TokenText;
                        next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace);
                    }

                    if (next != ParamTypes.SymLCurly)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }

                    context = new ParamClass(node, context, classname, superName, new List<IParamStatement>());
                    stack.Push(context);
                    continue;
                }

                case ParamTypes.AbsWhitespace:
                    break;
                case ParamTypes.EOF:

                default: throw new NotSupportedException(); //TODO: Error
            }

        }
        Done:
        {
            return Result.Merge(results);
        }

    }
}

