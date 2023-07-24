namespace BisUtils.Param.Parse;

using System.Text;
using Models.Stubs;
using Core.Parsing;
using Core.Parsing.Lexer;
using Enumerations;
using FResults;
using Lexer;
using Models;
using Models.Literals;
using Models.Statements;
using Models.Stubs.Holders;
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
                    do
                    {
                        next = lexer.NextToken();
                    } while (next == ParamTypes.AbsWhitespace);

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

                        next = lexer.NextToken();
                    }

                    if (next == ParamTypes.AbsWhitespace)
                    {
                        next = lexer.TokenizeWhile(ParamTypes.AbsWhitespace);
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
                    var value = ParseLiteral(node, null, lexer, ref next);

                    context.Statements.Add(new ParamVariable(node, context, name, value, operation));
                    if (lexer.PreviousChar == ';')
                    {
                        lexer.MoveBackward(2);
                    }

                    if (lexer.CurrentChar == ';')
                    {
                        lexer.MoveBackward();
                    }

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
    private static IParamLiteral ParseLiteral(IParamFile? file, IParamLiteralHolder? parent, ParamLexer lexer, ref IBisLexer<ParamTypes>.TokenMatch next)
    {
        do
        {
            lexer.MoveForward();
        } while (lexer.IsCurrentWhitespace);

        return lexer.CurrentChar == '{' ? ParseParamArray(file, parent, lexer) : ParseParamPrimitive(file, parent, lexer);
    }

    private static IParamLiteral ParseParamPrimitive(IParamFile? file, IParamLiteralHolder? parent, IBisStringStepper lexer, params char[] delimiters)
    {
        var value = ParseParamString(file, parent, lexer, delimiters);
        return value.ToFloat(out var paramFloat) ? paramFloat : value.ToInt(out var paramInt) ? paramInt : value;
    }

    private static ParamString ParseParamString(IParamFile? file, IParamLiteralHolder? parent, IBisStringStepper lexer, params char[] delimiters)
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
            return new ParamString(file, parent, builder.ToString(), quoted ? ParamStringType.Quoted : ParamStringType.Unquoted);
        }
    }

    private static ParamArray ParseParamArray(IParamFile? file, IParamLiteralHolder? parent, ParamLexer lexer)
    {
        var results = new List<Result>();
        var array = new ParamArray(file, parent, new List<IParamLiteral>());
        var stack = new Stack<ParamArray>();
        stack.Push(array);
        TraverseWhitespace(lexer);
        if(lexer.CurrentChar != '{')
        {
            results.Add(Result.Fail("Couldn't find array start."));
        }

        while (stack.Any())
        {

            var context = stack.Peek();
            lexer.MoveForward();
            TraverseWhitespace(lexer);
            switch (lexer.CurrentChar)
            {
                case '{':
                {
                    context.Value!.Add(ParseParamArray(file, context, lexer));
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
                    context.Value!.Add(ParseParamPrimitive(file, context, lexer, ';', '}', ','));
                    lexer.MoveBackward();
                    continue;
                }
            }
        }

        while (lexer.CurrentChar != '}')
        {
            lexer.MoveBackward();
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

