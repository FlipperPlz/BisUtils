namespace BisUtils.PreProcessor.RV;

using System.Text;
using Core.Parsing;
using Core.Parsing.Lexer;
using Enumerations;
using FResults;
using Models.Directives;
using Utils;

public interface IRVPreProcessor : IBisPreProcessor<RvTypes>
{
    List<IRVDefineDirective> MacroDefinitions { get; }

    RVIncludeFinder IncludeLocator { get; }


    IRVDefineDirective? LocateMacro(string name);
}

public class RVPreProcessor : BisPreProcessor<RvTypes>, IRVPreProcessor
{
    public List<IRVDefineDirective> MacroDefinitions { get; } = new ();
    public required RVIncludeFinder IncludeLocator { get; init; }
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

    private static readonly IBisLexer<RvTypes>.TokenDefinition DoubleQuoteDefinition =
        CreateTokenDefinition("rv.quote.double", RvTypes.SymDoubleQuote, 1);

    private static readonly IBisLexer<RvTypes>.TokenDefinition WhitespaceDefinition =
        CreateTokenDefinition("rv.whitespace", RvTypes.AbsWhitespace, 1);

    private static readonly IEnumerable<IBisLexer<RvTypes>.TokenDefinition> TokenDefinitions = new[]
    {
        EOFDefinition, TextDefinition, DHashDefinition, HashDefinition, CommaDefinition, LeftParenthesisDefinition,
        RightParenthesisDefinition, LeftAngleDefinition, RightAngleDefinition, DoubleQuoteDefinition, UndefDefinition,
        LineCommentDefinition, BlockCommentDefinition, DirectiveNewLineDefinition, NewLineDefinition, ElseDefinition,
        IfDefDefinition, IfNDefDefinition, EndifDefinition, IncludeDefinition, IdentifierDefinition
    };

    public override IEnumerable<IBisLexer<RvTypes>.TokenDefinition> TokenTypes => TokenDefinitions;
    public override IBisLexer<RvTypes>.TokenDefinition EOFToken => EOFDefinition;

    public RVPreProcessor(string content, List<IRVDefineDirective> macroDefinitions)
    {

    }

    protected override IBisLexer<RvTypes>.TokenMatch GetNextToken<T>(BisLexer<T> lexer)
    {
        lexer.MoveForward();

        var start = lexer.Position;
        if (IsWhitespace(lexer))
        {
            var builder = new StringBuilder(lexer.CurrentChar.ToString() ?? "");
            while (IsWhitespace(lexer, lexer.PeekForward()))
            {
                builder.Append(lexer.MoveForward());
            }

            return CreateTokenMatch(start..lexer.Position,builder.Remove(builder.Length, 1).ToString(), WhitespaceDefinition);
        }

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
                        return CreateTokenMatch(start..lineEnd, lexer.GetRange(start..(lineEnd + 1)), LineCommentDefinition);
                    case '*':
                    {
                        while (!(lexer.PreviousChar == '*' && lexer.CurrentChar == '/') && lexer.CurrentChar != null)
                        {
                        }

                        var blockEnd = lexer.Position;
                        return CreateTokenMatch(start..blockEnd, lexer.GetRange(start..(blockEnd + 1)), LineCommentDefinition);
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
                return CreateTokenMatch(start..lexer.Position, "\"", DoubleQuoteDefinition);
            case '\\':
            {
                if (lexer.PeekForward() == '\r')
                {
                    lexer.MoveForward();
                }

                if (lexer.PeekForward() == '\n')
                {
                    lexer.MoveForward();
                    return CreateTokenMatch(start..lexer.Position,"\\\n", DirectiveNewLineDefinition);
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
                    return CreateTokenMatch(start..lexer.Position,"undef", UndefDefinition);
                }

                break;
            }
        }

        if (!IsIdentifierChar(lexer, isFirst: true))
        {
            var text = lexer.ScanUntil(e => IsIdentifierChar(lexer, e), true);
            return CreateTokenMatch(start..lexer.Position, text, TextDefinition);
        }

        var id = lexer.ScanUntil(e => !IsIdentifierChar(lexer, e), true);
        return CreateTokenMatch(start..lexer.Position, id, LocateMacro(id) is not null ? IdentifierDefinition : TextDefinition);
    }

    public override Result EvaluateLexer<T>(BisLexer<T> lexer, StringBuilder? builder)
    {
        var result = new List<Result>();
        var quoted = false;

        IBisLexer<RvTypes>.TokenMatch token;
        while ((token = GetNextToken(lexer)) != RvTypes.SimEOF)
        {
            if (token == RvTypes.SymDoubleQuote)
            {
                quoted = !quoted;
            }

            if (quoted)
            {
                var next = GetNextToken(lexer);
                builder?.Append(next.TokenText);
                continue;
            }
            var isStart = token == RvTypes.AbsNewLine || PreviousMatch() is null;

            if (isStart)
            {
                builder?.Append('\n');
            }
            SkipWhitespaces(lexer);
            if (GetNextToken(lexer) == RvTypes.SimHash)
            {
                SkipWhitespaces(lexer);
                switch (GetNextToken(lexer).TokenType.TokenId)
                {
                    case RvTypes.KwInclude:
                    {
                        SkipWhitespaces(lexer);
                        token = GetNextToken(lexer);
                        if (token != RvTypes.SymDoubleQuote && token != RvTypes.SymLeftAngle)
                        {
                            result.Add(Result.Fail("Unknown character for include string, expected '<' or '\"'"));
                            goto End;
                        }

                        var end = token == RvTypes.SymDoubleQuote ? '"' : '>';
                        var path = lexer.ScanUntil(e => e == end);
                        var includeLocatorResult = IncludeLocator(path, builder);
                        result.Add(includeLocatorResult);

                        if (includeLocatorResult.IsFailed)
                        {
                            goto End;
                        }

                        break;
                    }
                    case RvTypes.KwDefine:
                    case RvTypes.KwIfDef:
                    case RvTypes.KwIfNDef:
                    case RvTypes.KwElse:
                    case RvTypes.KwEndIf:
                    case RvTypes.KwUndef:
                    {
                        //TODO:
                        throw new NotImplementedException();
                    }
                    case RvTypes.SimEOF:
                    case RvTypes.SimText:
                    case RvTypes.SimIdentifier:
                    case RvTypes.SimHash:
                    case RvTypes.SimDoubleHash:
                    case RvTypes.AbsWhitespace:
                    case RvTypes.SymLParenthesis:
                    case RvTypes.SymRParenthesis:
                    case RvTypes.SymComma:
                    case RvTypes.AbsNewLine:
                    case RvTypes.AbsLineComment:
                    case RvTypes.AbsBlockComment:
                    case RvTypes.AbsDirectiveNewLine:
                    case RvTypes.SymDoubleQuote:
                    case RvTypes.SymLeftAngle:
                    case RvTypes.SymRightAngle:
                    default:
                    {
                        result.Add(Result.Fail("Unknown preprocessor directive!"));
                        goto End;
                    }
                }

            }


        }



        End:
        {
            return Result.Merge(result);
        }
    }

    private void SkipWhitespaces(IBisStringStepper stepper)
    {
        while (stepper.PeekForward() is { } peeked && peeked < 33 && peeked != '\n')
        {
            stepper.MoveForward();
        }
    }

    private bool IsWhitespace(IBisStringStepper stepper, char? c = null) => (c ?? stepper.CurrentChar) switch
    {
        '\t' or '\u000B' or '\u000C' or ' ' or '\r' or '\n' => true,
        _ => false
    };


    public bool IsIdentifierChar(IBisStringStepper stepper, char? c = null, bool isFirst = false)
    {
        if ((c ?? stepper.CurrentChar) is not { } currentChar )
        {
            return false;
        }

        if (isFirst && char.IsAsciiDigit(currentChar))
        {
            return false;
        }

        return char.IsAsciiLetter(currentChar) || char.IsAsciiDigit(currentChar) || currentChar is '_';
    }

    public int TraverseLine(IBisStringStepper stepper)
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

}
