namespace BisUtils.Param.Parse;

using Models.Stubs;
using Core.Parsing;
using Core.Parsing.Lexer;
using Enumerations;
using FResults;
using Lexer;
using Models;
using Models.Literals;
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
                case ParamTypes.AbsWhitespace:
                {
                    continue;
                }
                case ParamTypes.EOF:
                {
                    if (stack.Count > 1)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    goto Done;
                }
                case ParamTypes.SymRCurly:
                {
                    if ((next = lexer.NextToken()) != ParamTypes.SymSeparator)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }

                    if (stack.Count == 1)
                    {
                        throw new NotSupportedException(); //TODO: Error no class to end
                    }

                    stack.Pop();
                    context = stack.Peek();
                    continue;
                }
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
                case ParamTypes.AbsIdentifier:
                {
                    var name = next.TokenText;
                    next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace);
                    var foundSquare = false;
                    if (next == ParamTypes.SymLSquare)
                    {
                        foundSquare = true;
                        if ((next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace)) != ParamTypes.SymRSquare)
                        {
                            throw new NotSupportedException(); //TODO: Error
                        }
                    }

                    if (next == ParamTypes.AbsWhitespace)
                    {
                        next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace);
                    }

                    var op = ((ParamTypes) next) switch
                    {
                        ParamTypes.OpAssign => ParamOperatorType.Assign,
                        ParamTypes.OpAddAssign => OperatorOf(foundSquare, true),
                        ParamTypes.OpSubAssign => OperatorOf(foundSquare, false),
                        _ => throw new NotSupportedException() //TODO: Error
                    };
                    context.Statements.Add(ParamVariable.CreateVariable(node, context, name, op,
                        ParseLiteral(lexer, out next)));
                    if ((next = lexer.NextToken()) != ParamTypes.SymSeparator)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    continue;
                }
                default: throw new NotSupportedException(); //TODO: Error
            }

        }
        Done:
        {
            return Result.Merge(results);
        }

    }

    private static IParamLiteralBase ParseLiteral(ParamLexer lexer, out IBisLexer<ParamTypes>.TokenMatch next)
    {
        var value = new ParamString((next = lexer.NextLiteral()).TokenText);
        if (next != ParamTypes.AbsLiteral)
        {
            throw new NotSupportedException(); //TODO: Error
        }

        return value.ToFloat(out var paramFloat) ? paramFloat
            : value.ToInt(out var paramInt) ? paramInt
            : value;
    }

    private static ParamOperatorType OperatorOf(bool isArray, bool isAddAssign)
    {
        if (!isArray)
        {
            throw new NotSupportedException(); //TODO: Error
        }
        return isAddAssign ? ParamOperatorType.AddAssign : ParamOperatorType.SubAssign;
    }
}

