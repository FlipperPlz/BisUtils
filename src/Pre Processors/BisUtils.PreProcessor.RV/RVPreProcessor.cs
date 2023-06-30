namespace BisUtils.PreProcessor.RV;

using System.Text;
using System.Text.RegularExpressions;
using Core.Parsing;
using Core.Parsing.Lexer;
using Enumerations;
using FResults;
using Models.Directives;
using Utils;

public interface IRVPreProcessor : IBisPreProcessorBase
{
    List<IRVDefineDirective> MacroDefinitions { get; }

    RVIncludeFinder IncludeLocator { get; }


    IRVDefineDirective? LocateMacro(string name);
}

public class RVPreProcessor : BisPreProcessor<RvTypes>, IRVPreProcessor
{
    public List<IRVDefineDirective> MacroDefinitions { get; } = new();
    public RVIncludeFinder IncludeLocator { get; init; } = DefaultIncludeLocator;

    public IRVDefineDirective? LocateMacro(string name) => MacroDefinitions.FirstOrDefault(e => e.MacroName == name);

    private static readonly IBisLexer<RvTypes>.TokenDefinition EOFDefinition =
        CreateTokenDefinition("rv.eof", RvTypes.SimEOF, 200);

    private static readonly IBisLexer<RvTypes>.TokenDefinition TextDefinition =
        CreateTokenDefinition("rv.text", RvTypes.SimText, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition IdentifierDefinition =
        CreateTokenDefinition("rv.identifier", RvTypes.SimIdentifier, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition DHashDefinition =
        CreateTokenDefinition("rv.hash.double", RvTypes.SimDoubleHash, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition HashDefinition =
        CreateTokenDefinition("rv.hash.single", RvTypes.SimHash, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition CommaDefinition =
        CreateTokenDefinition("rv.comma", RvTypes.SymComma, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition DefineDefinition =
        CreateTokenDefinition("rv.directive.define", RvTypes.KwDefine, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition IncludeDefinition =
        CreateTokenDefinition("rv.directive.include", RvTypes.KwInclude, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition NewLineDefinition =
        CreateTokenDefinition("rv.newLine", RvTypes.AbsNewLine, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition DirectiveNewLineDefinition =
        CreateTokenDefinition("rv.newLine.directive", RvTypes.AbsDirectiveNewLine, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition LineCommentDefinition =
        CreateTokenDefinition("rv.comment.line", RvTypes.AbsLineComment, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition BlockCommentDefinition =
        CreateTokenDefinition("rv.comment.block", RvTypes.AbsBlockComment, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition ElseDefinition =
        CreateTokenDefinition("rv.directive.else", RvTypes.KwElse, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition UndefDefinition =
        CreateTokenDefinition("rv.directive.undef", RvTypes.KwUndef, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition IfDefDefinition =
        CreateTokenDefinition("rv.directive.ifdef", RvTypes.KwIfDef, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition IfNDefDefinition =
        CreateTokenDefinition("rv.directive.ifdef.not", RvTypes.KwIfNDef, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition EndifDefinition =
        CreateTokenDefinition("rv.directive.endif", RvTypes.KwEndIf, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition LeftParenthesisDefinition =
        CreateTokenDefinition("rv.parenthesis.left", RvTypes.SymLParenthesis, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition RightParenthesisDefinition =
        CreateTokenDefinition("rv.parenthesis.right", RvTypes.SymRParenthesis, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition LeftAngleDefinition =
        CreateTokenDefinition("rv.angle.left", RvTypes.SymLeftAngle, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition RightAngleDefinition =
        CreateTokenDefinition("rv.angle.right", RvTypes.SymRightAngle, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition QuotedStringDefinition =
        CreateTokenDefinition("rv.string.quoted", RvTypes.AbsQuotedString, 1);


    private static readonly IBisLexer<RvTypes>.TokenDefinition WhitespaceDefinition =
        CreateTokenDefinition("rv.whitespace", RvTypes.AbsWhitespace, 1);

    private static readonly IEnumerable<IBisLexer<RvTypes>.TokenDefinition> TokenDefinitions = new[]
    {
        EOFDefinition, TextDefinition, DHashDefinition, HashDefinition, CommaDefinition, LeftParenthesisDefinition,
        RightParenthesisDefinition, LeftAngleDefinition, RightAngleDefinition, UndefDefinition,
        LineCommentDefinition, BlockCommentDefinition, DirectiveNewLineDefinition, NewLineDefinition,
        ElseDefinition, IfDefDefinition, IfNDefDefinition, EndifDefinition, IncludeDefinition, IdentifierDefinition,
        QuotedStringDefinition
    };

    public override IEnumerable<IBisLexer<RvTypes>.TokenDefinition> TokenTypes => TokenDefinitions;
    public override IBisLexer<RvTypes>.TokenDefinition EOFToken => EOFDefinition;

    public RVPreProcessor() => OnTokenMatched += HandlePreviousMatch;

    private void HandlePreviousMatch(IBisLexer<RvTypes>.TokenMatch match, IBisLexer<RvTypes> lexer) =>
        AddPreviousMatch(match);

    protected override IBisLexer<RvTypes>.TokenMatch GetNextToken(IBisMutableStringStepper lexer)
    {
        if (lexer.Length <= lexer.Position)
        {
            return CreateTokenMatch(..0, "", EOFDefinition);
        }

        lexer.MoveForward();
        var start = lexer.Position;

        switch (lexer.CurrentChar)
        {
            case null:
                return CreateTokenMatch(..0, "", EOFDefinition);
            case '\n':
                return CreateTokenMatch(start..lexer.Position, "\n", NewLineDefinition);
            case '\r':
            {
                if (lexer.PeekForward() == '\n')
                {
                    lexer.MoveForward();
                }

                return CreateTokenMatch(start..lexer.Position, "\r\n", NewLineDefinition);
            }
            case '/':
            {
                switch (lexer.PeekForward())
                {
                    case '/':
                        var lineEnd = lexer.Position + TraverseLine(lexer);
                        return CreateTokenMatch(start..lineEnd, lexer.GetRange(start..(lineEnd + 1)),
                            LineCommentDefinition);
                    case '*':
                    {
                        while (lexer is not { PreviousChar: '*', CurrentChar: '/' } && lexer.CurrentChar != null)
                        {
                            lexer.MoveForward();
                        }

                        var blockEnd = lexer.Position;
                        return CreateTokenMatch(start..blockEnd, lexer.GetRange(start..(blockEnd + 1)),
                            LineCommentDefinition);
                    }
                }

                break;
            }
            case '#':
            {
                if (lexer.PeekForward() != '#')
                {
                    return CreateTokenMatch(start..lexer.Position, "#", HashDefinition);
                }

                lexer.MoveForward();
                return CreateTokenMatch(start..lexer.Position, "##", DHashDefinition);
            }
            case '"':
            {
                var text = ScanRVString(lexer);
                return CreateTokenMatch(start..lexer.Position, text, QuotedStringDefinition);
            }
            case '\\':
            {
                if (lexer.PeekForward() == '\r')
                {
                    lexer.MoveForward();
                }

                if (lexer.PeekForward() == '\n')
                {
                    lexer.MoveForward();
                    return CreateTokenMatch(start..lexer.Position, "\\\n", DirectiveNewLineDefinition);
                }

                break;
            }
            case 'd':
            {
                if (lexer.PeekForwardMulti(6) == "define")
                {
                    lexer.MoveForward(5);
                    return CreateTokenMatch(start..lexer.Position, "define", DefineDefinition);
                }

                break;
            }
            case 'e':
            {
                switch (lexer.PeekForward())
                {
                    case 'l':
                    {
                        if (lexer.PeekForwardMulti(4) == "else")
                        {
                            lexer.MoveForward(3);
                            return CreateTokenMatch(start..lexer.Position, "else", ElseDefinition);
                        }

                        break;
                    }
                    case 'n':
                    {
                        if (lexer.PeekForwardMulti(5) == "endif")
                        {
                            lexer.MoveForward(4);
                            return CreateTokenMatch(start..lexer.Position, "endif", ElseDefinition);
                        }

                        break;
                    }
                }

                break;
            }
            case 'i':
            {
                switch (lexer.PeekForward())
                {
                    case 'f':
                    {
                        lexer.MoveForward();
                        switch (lexer.MoveForward())
                        {
                            case 'n':
                            {
                                if (lexer.PeekForwardMulti(4) == "ndef")
                                {
                                    lexer.MoveForward(3);
                                    return CreateTokenMatch(start..lexer.Position, "ifndef", IfNDefDefinition);
                                }

                                break;
                            }
                            case 'd':
                            {
                                if (lexer.PeekForwardMulti(3) == "def")
                                {
                                    lexer.MoveForward(2);
                                    return CreateTokenMatch(start..lexer.Position, "ifdef", IfDefDefinition);
                                }

                                break;
                            }
                        }

                        break;
                    }
                    case 'n':
                    {
                        if (lexer.PeekForwardMulti(7) == "include")
                        {
                            lexer.MoveForward(6);
                            return CreateTokenMatch(start..lexer.Position, "include", IncludeDefinition);
                        }

                        break;
                    }
                }

                break;
            }
            case 'u':
            {
                if (lexer.PeekForwardMulti(5) == "undef")
                {
                    lexer.MoveForward(4);
                    return CreateTokenMatch(start..lexer.Position, "undef", UndefDefinition);
                }

                break;
            }
            case ' ' or '\t':
            {
                var builder = new StringBuilder().Append(lexer.CurrentChar);
                while (lexer.PeekForward() is ' ' or '\t')
                {
                    builder.Append(lexer.MoveForward());
                }

                return CreateTokenMatch(start..lexer.Position, builder.ToString(), WhitespaceDefinition);
            }
        }

        if (IsWhitespace(lexer))
        {
            while (IsWhitespace(lexer, lexer.PeekForward()))
            {
                lexer.MoveForward();
            }

            return CreateTokenMatch(start..lexer.Position, "", WhitespaceDefinition);
        }


        var id = lexer.ScanUntil(e => !IsIdentifierChar(lexer, e), true);
        return CreateTokenMatch(start..lexer.Position, id,
            LocateMacro(id) is not null ? IdentifierDefinition : TextDefinition);
    }

    public override Result EvaluateLexer(IBisMutableStringStepper lexer, StringBuilder? builder)
    {
        var result = new List<Result>();

        IBisLexer<RvTypes>.TokenMatch token;
        while ((token = NextToken(lexer)) != RvTypes.SimEOF)
        {
            switch (token.TokenType.TokenId)
            {
                case RvTypes.SimHash:
                    result.Add(ProcessDirective(lexer, builder));
                    break;
                case RvTypes.AbsWhitespace:
                    ProcessWhitespace(builder);
                    break;
                case RvTypes.AbsDirectiveNewLine:
                    ProcessNewLine(builder, true);
                    break;
                case RvTypes.AbsNewLine:
                    ProcessNewLine(builder, false);
                    break;
                case RvTypes.AbsLineComment:
                    ProcessComment(builder, true, token.TokenText);
                    break;
                case RvTypes.AbsBlockComment:
                    ProcessComment(builder, false, token.TokenText);
                    break;
                case RvTypes.SimIdentifier:
                    break; //TODO: look for arguments; Evaluate macro
                case RvTypes.SimDoubleHash:
                    break; //TODO process next token as macro
                // DEFAULT WRITE TO OUTPUT
                case RvTypes.KwInclude:
                case RvTypes.KwDefine:
                case RvTypes.KwIfDef:
                case RvTypes.KwIfNDef:
                case RvTypes.KwElse:
                case RvTypes.KwEndIf:
                case RvTypes.KwUndef:
                case RvTypes.SymComma:
                case RvTypes.AbsQuotedString:
                case RvTypes.SymLeftAngle:
                case RvTypes.SymRightAngle:
                case RvTypes.SimText:
                case RvTypes.SimEOF:
                case RvTypes.SymLParenthesis:
                case RvTypes.SymRParenthesis:
                default:
                    builder?.Append(token.TokenText);
                    break;
            }
        }

        builder?.Replace("\n\n", "\n").Replace("\r\n\r\n", "\n");

        return Result.Merge(result);
    }

    private static string ScanRVString(IBisStringStepper lexer)
    {
        var builder = new StringBuilder().Append('"');
        do
        {
            var next = lexer.MoveForward();
            if (next == '\\')
            {
                if (lexer.PeekForward() == '\r')
                {
                    lexer.MoveForward();
                }

                builder.Append(lexer.PeekForward() != '\n' ? '\\' : '\n');
                continue;
            }

            builder.Append(next);
        } while (lexer.CurrentChar != '"' && lexer.CurrentChar != null);

        return builder.ToString();
    }


    private static void SkipWhitespaces(IBisStringStepper stepper)
    {
        while (stepper.PeekForward() is { } peeked && peeked < 33 && peeked != '\n')
        {
            stepper.MoveForward();
        }
    }

    private static bool IsWhitespace(IBisStringStepper stepper, char? c = null) => (c ?? stepper.CurrentChar) switch
    {
        '\t' or '\u000B' or '\u000C' or ' ' or '\r' or '\n' => true,
        _ => false
    };


    private static bool IsIdentifierChar(IBisStringStepper stepper, char? c = null, bool isFirst = false)
    {
        if ((c ?? stepper.CurrentChar) is not { } currentChar)
        {
            return false;
        }

        if (isFirst && char.IsAsciiDigit(currentChar))
        {
            return false;
        }

        return char.IsAsciiLetter(currentChar) || char.IsAsciiDigit(currentChar) || currentChar is '_';
    }

    private static int TraverseLine(IBisStringStepper stepper)
    {
        var charCount = 0;
        while (true)
        {
            switch (stepper.CurrentChar)
            {
                case null:
                    return charCount;
                case '\n':
                    stepper.MoveForward();
                    return ++charCount;
                default:
                    charCount++;
                    stepper.MoveForward();
                    break;
            }
        }
    }

    protected virtual void ProcessComment(StringBuilder? builder, bool lineComment, string commentText)
    {
    }

    protected virtual Result ProcessDirective(IBisMutableStringStepper lexer, StringBuilder? builder)
    {
        SkipWhitespaces(lexer);
        return NextToken(lexer).TokenType.TokenId switch
        {
            RvTypes.KwInclude => ProcessIncludeDirective(lexer, builder),
            //TODO:
            _ => Result.Fail("Unknown preprocessor directive!")
        };
    }

    protected virtual Result ProcessIncludeDirective(IBisStringStepper lexer, StringBuilder? builder)
    {
        SkipWhitespaces(lexer);
        lexer.MoveForward();
        var token = lexer.CurrentChar;
        if (token != '"' && token != '<')
        {
            return Result.Fail("Unknown character for include string, expected '<' or '\"'");
        }

        var end = token == '"' ? '"' : '>';
        var path = lexer.ScanUntil(e => e == end);
        lexer.MoveForward();
        return IncludeLocator(path, builder);
    }

    protected virtual void ProcessWhitespace(StringBuilder? builder) => builder?.Append(' ');

    protected virtual void ProcessNewLine(StringBuilder? builder, bool directiveNewLine)
    {
        if (!directiveNewLine)
        {
            builder?.Append('\n');
        }
    }

    protected static Result DefaultIncludeLocator(string filepath, StringBuilder? builder) => Result.Ok();

}
