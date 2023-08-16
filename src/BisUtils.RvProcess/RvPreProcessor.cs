namespace BisUtils.RvProcess;

using System.Text;
using Core.Parsing.Lexer;
using Enumerations;
using FResults;
using FResults.Reasoning;
using Models.Directives;
using Models.Elements;
using Models.Stubs;
using Utils;

/// <summary>
/// Interface for a preprocessor specifically designed for RV programming language.
/// </summary>
public interface IRvPreProcessor : IBisPreProcessorBase
{
    /// <summary>
    /// Event that fires when a token match occurs.
    /// </summary>
    public event DirectiveMatched OnDirectiveMatched;

    /// <summary>
    /// Delegate for a parsing event fired when a directive is matched.
    /// </summary>
    /// <param name="directive">The directive matched.</param>
    /// <param name="replacement">What the directive should be replaced with (may already be set)</param>
    /// <param name="processor">The processor instance handling the input.</param>
    public delegate IEnumerable<IReason> DirectiveMatched(IRvDirective directive, IRvPreProcessor processor, ref string replacement);

    /// <summary>
    /// A list of directives that have been matched
    /// </summary>
    Dictionary<bool, IRvDirective> MatchedDirectives { get; }

    /// <summary>
    /// A list of macro definitions available in the preprocessor.
    /// </summary>
    List<IRvDefineDirective> MacroDefinitions { get; }

    /// <summary>
    /// Responsible for locating include files.
    /// </summary>
    RVIncludeFinder IncludeLocator { get; }

    /// <summary>
    /// Locates a macro by its name.
    /// </summary>
    /// <param name="name">Name of the macro to locate.</param>
    /// <returns>The macro if it was found, otherwise null.</returns>
    IRvDefineDirective? LocateMacro(string name);
}

/// <summary>
/// Implementation of a preprocessor for the RV programming language.
/// <see cref="BisPreProcessor{TPreProcTypes}"/>
/// </summary>
public class RvPreProcessor : BisPreProcessor<RvTypes>, IRvPreProcessor
{
    public event IRvPreProcessor.DirectiveMatched? OnDirectiveMatched;

    public Dictionary<bool, IRvDirective> MatchedDirectives { get; } = new();

    /// <summary>
    /// A list of macro definitions available in the preprocessor.
    /// </summary>
    public List<IRvDefineDirective> MacroDefinitions { get; } = new();



    /// <summary>
    /// Responsible for locating include files.
    /// </summary>
    public RVIncludeFinder IncludeLocator { get; init; } = DefaultIncludeLocator;

    /// <summary>
    /// Locates a macro by its name.
    /// </summary>
    /// <param name="name">Name of the macro to locate.</param>
    /// <returns>The macro if it was found, otherwise null.</returns>
    public IRvDefineDirective? LocateMacro(string name) => MacroDefinitions.FirstOrDefault(e => e.MacroName == name);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition EOFDefinition =
        CreateTokenDefinition("rv.eof", RvTypes.SimEOF, 200);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition TextDefinition =
        CreateTokenDefinition("rv.text", RvTypes.SimText, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition IdentifierDefinition =
        CreateTokenDefinition("rv.identifier", RvTypes.SimIdentifier, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition DHashDefinition =
        CreateTokenDefinition("rv.hash.double", RvTypes.SimDoubleHash, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition HashDefinition =
        CreateTokenDefinition("rv.hash.single", RvTypes.SimHash, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition CommaDefinition =
        CreateTokenDefinition("rv.comma", RvTypes.SymComma, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition DefineDefinition =
        CreateTokenDefinition("rv.directive.define", RvTypes.KwDefine, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition IncludeDefinition =
        CreateTokenDefinition("rv.directive.include", RvTypes.KwInclude, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition NewLineDefinition =
        CreateTokenDefinition("rv.newLine", RvTypes.AbsNewLine, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition DirectiveNewLineDefinition =
        CreateTokenDefinition("rv.newLine.directive", RvTypes.AbsDirectiveNewLine, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition LineCommentDefinition =
        CreateTokenDefinition("rv.comment.line", RvTypes.AbsLineComment, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition BlockCommentDefinition =
        CreateTokenDefinition("rv.comment.block", RvTypes.AbsBlockComment, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition ElseDefinition =
        CreateTokenDefinition("rv.directive.else", RvTypes.KwElse, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition UndefDefinition =
        CreateTokenDefinition("rv.directive.undef", RvTypes.KwUndef, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition IfDefDefinition =
        CreateTokenDefinition("rv.directive.ifdef", RvTypes.KwIfDef, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition IfNDefDefinition =
        CreateTokenDefinition("rv.directive.ifdef.not", RvTypes.KwIfNDef, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition EndifDefinition =
        CreateTokenDefinition("rv.directive.endif", RvTypes.KwEndIf, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition LeftParenthesisDefinition =
        CreateTokenDefinition("rv.parenthesis.left", RvTypes.SymLParenthesis, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition RightParenthesisDefinition =
        CreateTokenDefinition("rv.parenthesis.right", RvTypes.SymRParenthesis, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition LeftAngleDefinition =
        CreateTokenDefinition("rv.angle.left", RvTypes.SymLeftAngle, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition RightAngleDefinition =
        CreateTokenDefinition("rv.angle.right", RvTypes.SymRightAngle, 1);

    private static readonly IBisLexerOld<RvTypes>.TokenDefinition QuotedStringDefinition =
        CreateTokenDefinition("rv.string.quoted", RvTypes.AbsQuotedString, 1);


    private static readonly IBisLexerOld<RvTypes>.TokenDefinition WhitespaceDefinition =
        CreateTokenDefinition("rv.whitespace", RvTypes.AbsWhitespace, 1);

    private static readonly IEnumerable<IBisLexerOld<RvTypes>.TokenDefinition> TokenDefinitions = new[]
    {
        EOFDefinition, TextDefinition, DHashDefinition, HashDefinition, CommaDefinition, LeftParenthesisDefinition,
        RightParenthesisDefinition, LeftAngleDefinition, RightAngleDefinition, UndefDefinition,
        LineCommentDefinition, BlockCommentDefinition, DirectiveNewLineDefinition, NewLineDefinition,
        ElseDefinition, IfDefDefinition, IfNDefDefinition, EndifDefinition, IncludeDefinition, IdentifierDefinition,
        QuotedStringDefinition
    };

    public override IEnumerable<IBisLexerOld<RvTypes>.TokenDefinition> TokenTypes => TokenDefinitions;
    public override IBisLexerOld<RvTypes>.TokenDefinition EOFToken => EOFDefinition;

    /// <summary>
    /// Initializes a new instance of the <see cref="RvPreProcessor"/> class.
    /// </summary>
    public RvPreProcessor()
    {
        OnTokenMatched += HandlePreviousMatch;
        OnDirectiveMatched += EvaluateDirective;
    }


    private IEnumerable<IReason> EvaluateDirective(IRvDirective directive, IRvPreProcessor processor,
        ref string replacement) =>
        directive switch
        {
            IRvIncludeDirective includeDirective => IncludeLocator(includeDirective, ref replacement),
            _ => new[] { new Error() }
        };


    /// <summary>
    /// Handles the event of a token match.
    /// </summary>
    /// <param name="match">matched token</param>
    /// <param name="lexerOld">lexer instance</param>
    private void HandlePreviousMatch(IBisLexerOld<RvTypes>.TokenMatch match, IBisLexerOld<RvTypes> lexerOld) =>
        AddPreviousMatch(match);

    protected override IBisLexerOld<RvTypes>.TokenMatch GetNextToken(IBisMutableStringStepper lexer)
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
                        lexer.MoveBackward();
                        return CreateTokenMatch(start..lineEnd, lexer.GetRange(start..lineEnd),
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

    /// <summary>
    /// Evaluates the lexer and performs respective actions based on the type of token found.
    /// </summary>
    /// <param name="lexer">The active lexer to analyze.</param>
    /// <param name="builder">A StringBuilder instance which may be used to create/modify strings during the evaluation process.</param>
    /// <returns>Result object containing the results of the lexer evaluation.</returns>
    public override Result EvaluateLexer(IBisMutableStringStepper lexer, StringBuilder? builder)
    {
        var result = Result.ImmutableOk();

        IBisLexerOld<RvTypes>.TokenMatch token;
        while ((token = NextToken(lexer)) != RvTypes.SimEOF)
        {
            switch (token.TokenType.TokenId)
            {
                case RvTypes.SimHash:
                    result.Reasons.AddRange(ProcessDirective(lexer, builder));
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

    protected virtual IEnumerable<IReason> ProcessDirective(IBisMutableStringStepper lexer, StringBuilder? builder)
    {
        SkipWhitespaces(lexer);
        return NextToken(lexer).TokenType.TokenId switch
        {
            RvTypes.KwInclude => ProcessIncludeDirective(lexer, builder),
            RvTypes.KwUndef => ProcessUndefineDirective(lexer, builder),
            //TODO: Include remaining directives
            _ => new[] { new Error() { Message = "Unknown preprocessor directive!" } }
        };
    }

    private IEnumerable<IReason> ProcessUndefineDirective(IBisMutableStringStepper lexer, StringBuilder? builder)
    {
        var next = NextToken(lexer);
        while (next == RvTypes.AbsWhitespace)
        {
            next = NextToken(lexer);
        }

        if (next != RvTypes.SimIdentifier)
        {
            return new[] { new Error() { Message = "Unable to read '#undef' an identifier wasn't found!" } };
        }

        var replacement = "";
        return OnDirectiveMatchedHandler
        (
            new RvUndefineDirective(this, next.TokenText),
            ref replacement,
            builder
        );
    }

    protected virtual IEnumerable<IReason> ProcessIncludeDirective(IBisStringStepper lexer, StringBuilder? builder)
    {
        SkipWhitespaces(lexer);
        lexer.MoveForward();
        var token = lexer.CurrentChar;
        if (token != '"' && token != '<')
        {
            return new[] { new Error() { Message = "Unknown character for include string, expected '<' or '\"'" } };
        }

        var end = token == '"' ? '"' : '>';
        var path = lexer.ScanUntil(e => e == end);
        var replacement = "";
        return OnDirectiveMatchedHandler
        (
            new RvIncludeDirective
            (
                this,
                new RvIncludeString
                (
                    this,
                    path,
                    end == '"' ? RvStringType.Quoted : RvStringType.Angled
                )
            ),
            ref replacement,
            builder
        );
    }


    protected virtual IEnumerable<IReason> OnDirectiveMatchedHandler(IRvDirective directive, ref string replacement, StringBuilder? builder)
    {
        var result = OnDirectiveMatched?.Invoke(directive, this, ref replacement);
        builder?.Append(replacement);
        return result ?? Array.Empty<IReason>();
    }

    protected virtual void ProcessWhitespace(StringBuilder? builder) => builder?.Append(' ');

    protected virtual void ProcessNewLine(StringBuilder? builder, bool directiveNewLine)
    {
        if (!directiveNewLine)
        {
            builder?.Append('\n');
        }

    }

    private static IEnumerable<IReason>
        DefaultIncludeLocator(IRvIncludeDirective include, ref string includeContents) => Array.Empty<IReason>();

}
