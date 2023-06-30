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

/// <summary>
/// The `ParamParser` class implements the `IBisParser` interface for parsing parameter files.
/// It specifically parses `ParamFile` nodes using the `ParamLexer` and `ParamTypes` types.
/// It also uses `RVPreProcessor` for preprocessing.
/// </summary>
public class ParamParser : IBisParser<ParamFile, ParamLexer, ParamTypes, RVPreProcessor>
{
    /// <summary>
    /// Gets the current and only instance of the parser.
    /// </summary>
    public static readonly ParamParser Instance = new();

    private ParamParser()
    {
    }

#pragma warning disable CA1822
    /// <summary>
    /// Parses a `ParamLexer` instance into a `ParamFile` node.
    /// It supports parsing different parameter types like class
    /// definitions, deletions, enums, and identifier assignment
    /// with corresponding behaviors. This method throws a
    /// `NotSupportedException` for unexpected tokens or syntax.
    /// </summary>
    /// <param name="node">The output `ParamFile` node.</param>
    /// <param name="lexer">The `ParamLexer` instance to be parsed.</param>
    /// <returns>A `Result` instance representing the result of the parsing operation.</returns>
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
                    switch ((ParamTypes) (next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace)))
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
                        if ((next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace)) != ParamTypes.AbsIdentifier)
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
                case ParamTypes.KwDelete:
                {
                    next = lexer.NextToken();
                    if (next != ParamTypes.AbsWhitespace)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    if ((next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace)) != ParamTypes.AbsIdentifier)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }

                    if ((next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace)) != ParamTypes.SymSeparator)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    context.Statements.Add(new ParamDelete(node, context, next.TokenText));

                    continue;
                }
                case ParamTypes.KwEnum:
                {
                    if (lexer.NextToken() != ParamTypes.SymLCurly)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    //TODO: Handle Enum:
                    lexer.TokenizeWhile(ParamTypes.SymRCurly);
                    if (lexer.TokenizeWhile(ParamTypes.AbsWhitespace) != ParamTypes.SymSeparator)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    break;
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

                    context.Statements.Add(ParamVariable.CreateVariable(node, context, name, (ParamTypes) next switch
                    {
                        ParamTypes.OpAssign => ParamOperatorType.Assign,
                        ParamTypes.OpAddAssign => foundSquare ? OperatorOf(true) : throw new NotSupportedException(), //TODO ERROR
                        ParamTypes.OpSubAssign => foundSquare ? OperatorOf(false): throw new NotSupportedException(),//TODO ERROR
                        _ => throw new NotSupportedException() //TODO: Error
                    },ParseLiteral(lexer, out next)));
                    if (lexer.NextToken() != ParamTypes.SymSeparator)
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

    /// <summary>
    /// Parses a literal into `IParamLiteralBase` instance which could be a `ParamFloat`,
    /// `ParamInt` or `ParamString` depending on the literal token found.
    /// This method throws a `NotSupportedException` for unexpected or invalid literal tokens.
    /// </summary>
    /// <param name="lexer">The `ParamLexer` providing the literal to parse.</param>
    /// <param name="next">Output parameter to hand back the next token after parsing the literal.</param>
    /// <returns>An `IParamLiteralBase` instance representing the parsed literal.</returns>
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

    /// <summary>
    /// Determines the `ParamOperatorType` based on whether the parsed item is an AddAssign or SubAssign operator.
    /// </summary>
    /// <param name="isAddAssign">Boolean indicator to denote whether the operator is an AddAssign or not.</param>
    /// <returns>A `ParamOperatorType` value representing the operator.</returns>
    private static ParamOperatorType OperatorOf(bool isAddAssign) => isAddAssign ? ParamOperatorType.AddAssign : ParamOperatorType.SubAssign;
}

