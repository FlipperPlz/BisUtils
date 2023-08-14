namespace BisUtils.RvConfig.Parse;

using System.Text;
using Models.Stubs;
using Core.Parsing;
using Core.Parsing.Lexer;
using Enumerations;
using FResults;
using Lexer;
using Microsoft.Extensions.Logging;
using Models;
using Models.Literals;
using Models.Statements;
using Models.Stubs.Holders;
using RvProcess;

/// <summary>
/// The `ParamParser` class implements the `IBisParser` interface for parsing parameter files.
/// It specifically parses `ParamFile` nodes using the `ParamLexer` and `ParamTypes` types.
/// It also uses `RVPreProcessor` for preprocessing.
/// </summary>
public class ParamParserOld : IBisParserOld<ParamFile, ParamLexerOld, ParamTypes, RvPreProcessor>
{
    /// <summary>
    /// Gets the current and only instance of the parser.
    /// </summary>
    public static readonly ParamParserOld Instance = new();

    private ParamParserOld()
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
    /// <param name="lexerOld">The `ParamLexer` instance to be parsed.</param>
    /// <param name="logger">Logger for parsing</param>
    /// <returns>A `Result` instance representing the result of the parsing operation.</returns>
    public Result Parse(out ParamFile? node, ParamLexerOld lexerOld, ILogger? logger)
    {
        var results = new List<Result>();
        node = new ParamFile("config", new List<IParamStatement>(), logger);
        var stack = new Stack<IParamStatementHolder>();
        stack.Push(node);
        var context = stack.Peek();
        while (stack.Any())
        {
            var next = lexerOld.NextToken();
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
                    if ((next = lexerOld.NextToken()) != ParamTypes.SymSeparator)
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
                    next = lexerOld.NextToken();
                    if (next != ParamTypes.AbsWhitespace)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }

                    next = lexerOld.NextToken();
                    if (next != ParamTypes.AbsIdentifier)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    var classname = next.TokenText;
                    do
                    {
                        next = lexerOld.NextToken();
                    } while (next == ParamTypes.AbsWhitespace);

                    switch ((ParamTypes) next)
                    {
                        case ParamTypes.SymSeparator:
                        {
                            context.Statements.Add(new ParamExternalClass(classname, node, context, logger));
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
                        if ((next = lexerOld.TokenizeWhile(ParamTypes.AbsWhitespace)) != ParamTypes.AbsIdentifier)
                        {
                            throw new NotSupportedException(); //TODO: Error
                        }

                        superName = next.TokenText;
                        next = lexerOld.TokenizeWhile(ParamTypes.AbsWhitespace);
                    }

                    if (next != ParamTypes.SymLCurly)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }

                    context = new ParamClass(classname, superName, new List<IParamStatement>(), node, context, logger);
                    stack.Push(context);
                    continue;
                }
                case ParamTypes.KwDelete:
                {
                    next = lexerOld.NextToken();
                    if (next != ParamTypes.AbsWhitespace)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    if ((next = lexerOld.TokenizeWhile(ParamTypes.AbsWhitespace)) != ParamTypes.AbsIdentifier)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }

                    if ((next = lexerOld.TokenizeWhile(ParamTypes.AbsWhitespace)) != ParamTypes.SymSeparator)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    context.Statements.Add(new ParamDelete(next.TokenText, node, context, logger));

                    continue;
                }
                case ParamTypes.KwEnum:
                {
                    if (lexerOld.NextToken() != ParamTypes.SymLCurly)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    //TODO: Handle Enum:
                    lexerOld.TokenizeWhile(ParamTypes.SymRCurly);
                    if (lexerOld.TokenizeWhile(ParamTypes.AbsWhitespace) != ParamTypes.SymSeparator)
                    {
                        throw new NotSupportedException(); //TODO: Error
                    }
                    break;
                }
                case ParamTypes.AbsIdentifier:
                {
                    var name = next.TokenText;
                    next = lexerOld.TokenizeWhile(ParamTypes.AbsWhitespace);
                    var foundSquare = false;
                    if (next == ParamTypes.SymLSquare)
                    {
                        foundSquare = true;
                        if ((next = lexerOld.TokenizeWhile(ParamTypes.AbsWhitespace)) != ParamTypes.SymRSquare)
                        {
                            throw new NotSupportedException(); //TODO: Error
                        }

                        next = lexerOld.NextToken();
                    }

                    if (next == ParamTypes.AbsWhitespace)
                    {
                        next = lexerOld.TokenizeWhile(ParamTypes.AbsWhitespace);
                    }

                    var operation = (ParamTypes)next switch
                    {
                        ParamTypes.OpAssign => ParamOperatorType.Assign,
                        ParamTypes.OpAddAssign => foundSquare
                            ? OperatorOf(true)
                            : throw new NotSupportedException(), //TODO ERROR
                        ParamTypes.OpSubAssign => foundSquare
                            ? OperatorOf(false)
                            : throw new NotSupportedException(), //TODO ERROR
                        _ => throw new NotSupportedException() //TODO: Error
                    };
                    var value = ParseLiteral(node, null, logger, lexerOld, ref next);

                    context.Statements.Add(new ParamVariable(name, value, operation, node, context, logger));
                    if (lexerOld.PreviousChar == ';')
                    {
                        lexerOld.MoveBackward(2);
                    }

                    if (lexerOld.CurrentChar == ';')
                    {
                        lexerOld.MoveBackward();
                    }

                    if (lexerOld.NextToken() != ParamTypes.SymSeparator)
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
    /// <param name="lexerOld">The `ParamLexer` providing the literal to parse.</param>
    /// <param name="next">Output parameter to hand back the next token after parsing the literal.</param>
    /// <returns>An `IParamLiteralBase` instance representing the parsed literal.</returns>
    private static IParamLiteral ParseLiteral(IParamFile file, IParamLiteralHolder parent, ILogger? logger, ParamLexerOld lexerOld, ref IBisLexerOld<ParamTypes>.TokenMatch next)
    {
        do
        {
            lexerOld.MoveForward();
        } while (lexerOld.IsCurrentWhitespace);

        return lexerOld.CurrentChar == '{' ? ParseParamArray(file, parent, logger, lexerOld) : ParseParamPrimitive(file, parent, logger, lexerOld);
    }

    private static IParamLiteral ParseParamPrimitive(IParamFile? file, IParamLiteralHolder? parent, ILogger? logger, IBisStringStepper lexer, params char[] delimiters)
    {
        var value = ParseParamString(file, parent, logger, lexer, delimiters);
        return value.ToFloat(out var paramFloat) ? paramFloat : value.ToInt(out var paramInt) ? paramInt : value;
    }

    private static ParamString ParseParamString(IParamFile file, IParamLiteralHolder parent, ILogger? logger, IBisStringStepper? lexer, params char[] delimiters)
    {
        var quoted = false;
        if (lexer.CurrentChar == '"')
        {
            quoted = true;
            lexer.MoveForward();
        }

        var builder = new StringBuilder();

        while (lexer.CurrentChar is { } currentChar && !delimiters.Contains(currentChar))
        {
            switch (currentChar)
            {
                case '\n' or '\r':
                    goto Finish;
                case ';' when !quoted:
                {
                    goto Finish;
                }
                case '"' when quoted:
                {
                    if (lexer.MoveForward() != '"')
                    {
                        if (currentChar != '\\')
                        {
                            goto Finish;
                        }

                        if (lexer.MoveForward() != 'n')
                        {

                            goto Finish;
                        }
                        if (lexer.CurrentChar != '"')
                        {
                            goto Finish;
                        }

                        builder.Append('\n');
                        lexer.MoveForward();
                    }

                    break;
                }
            }

            builder.Append(lexer.CurrentChar);
            lexer.MoveForward();
        }
        Finish:
        {
            return new ParamString(builder.ToString(), quoted ? ParamStringType.Quoted : ParamStringType.Unquoted, file, parent,  logger);
        }
    }

    private static ParamArray ParseParamArray(IParamFile file, IParamLiteralHolder parent, ILogger? logger, ParamLexerOld lexerOld)
    {
        var results = new List<Result>();
        var array = new ParamArray(new List<IParamLiteral>(), file, parent, logger);
        var stack = new Stack<ParamArray>();
        stack.Push(array);
        TraverseWhitespace(lexerOld);
        if(lexerOld.CurrentChar != '{')
        {
            results.Add(Result.Fail("Couldn't find array start."));
        }

        while (stack.Any())
        {

            var context = stack.Peek();
            lexerOld.MoveForward();
            TraverseWhitespace(lexerOld);
            switch (lexerOld.CurrentChar)
            {
                case '{':
                {
                    context.Value!.Add(ParseParamArray(file, context, logger, lexerOld));
                    continue;
                }
                case ',': continue;
                case '}':
                {
                    stack.Pop();
                    continue;
                }
                default:
                {
                    context.Value!.Add(ParseParamPrimitive(file, context, logger, lexerOld, ';', '}', ','));
                    lexerOld.MoveBackward();
                    continue;
                }
            }
        }

        while (lexerOld.CurrentChar != '}')
        {
            lexerOld.MoveBackward();
        }
        return array;
    }

    private static readonly char[] Whitespaces = { ' ', '\t', '\u000B', '\u000C' };
    private static int TraverseWhitespace(IBisStringStepper stepper, bool allowEOF = false, bool allowEOL = true)
    {
        var charCount = 0;
        while (true)
        {
            if (stepper.IsEOF())
            {
                if (allowEOF)
                {
                    break;
                }

                return charCount;
            }

            switch (stepper.CurrentChar)
            {
                case '\r':
                {
                    if (!allowEOL)
                    {
                        return charCount;
                    }

                    charCount += stepper.MoveForward() == '\n' ? 2 : 1;
                    break;
                }
                case '\n':
                {
                    if (!allowEOL)
                    {
                        return charCount;
                    }

                    charCount++;
                    stepper.MoveForward();
                    break;
                }
                default:
                {
                    if (stepper.CurrentChar is { } current)
                    {
                        if (!Whitespaces.Contains(current))
                        {
                            return charCount;
                        }

                        stepper.MoveForward();
                        charCount++;
                    }
                    else
                    {
                        if (allowEOF)
                        {
                            break;
                        }

                        return charCount;
                    }

                    break;
                }
            }
        }

        return charCount;
    }

    /// <summary>
    /// Determines the `ParamOperatorType` based on whether the parsed item is an AddAssign or SubAssign operator.
    /// </summary>
    /// <param name="isAddAssign">Boolean indicator to denote whether the operator is an AddAssign or not.</param>
    /// <returns>A `ParamOperatorType` value representing the operator.</returns>
    private static ParamOperatorType OperatorOf(bool isAddAssign) => isAddAssign ? ParamOperatorType.AddAssign : ParamOperatorType.SubAssign;
}

