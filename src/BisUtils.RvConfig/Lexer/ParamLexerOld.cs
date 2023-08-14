namespace BisUtils.RvConfig.Lexer;

using Core.Parsing.Lexer;

public class ParamLexerOld : BisLexerOld<ParamTypes>
{
    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition ErrorDefinition =
        CreateTokenDefinition("param.error", ParamTypes.Invalid, -1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition EOFDefinition =
        CreateTokenDefinition("param.eof", ParamTypes.EOF, 200);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition LiteralDefinition =
        CreateTokenDefinition("param.abs.literal", ParamTypes.AbsLiteral, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition WhitespaceDefinition =
        CreateTokenDefinition("param.abs.whitespace", ParamTypes.AbsWhitespace, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition PreProcessDefinition =
        CreateTokenDefinition("param.abs.preprocess", ParamTypes.AbsPreprocess, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition IdentifierDefinition =
        CreateTokenDefinition("param.abs.identifier", ParamTypes.AbsIdentifier, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition ClassDefinition =
        CreateTokenDefinition("param.keyword.class", ParamTypes.KwClass, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition EnumDefinition =
        CreateTokenDefinition("param.keyword.enum", ParamTypes.KwEnum, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition DeleteDefinition =
        CreateTokenDefinition("param.keyword.delete", ParamTypes.KwDelete, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition LCurlyDefinition =
        CreateTokenDefinition("param.symbol.curly.left", ParamTypes.SymLCurly, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition RCurlyDefinition =
        CreateTokenDefinition("param.symbol.curly.right", ParamTypes.SymRCurly, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition LSquareDefinition =
        CreateTokenDefinition("param.symbol.square.left", ParamTypes.SymLSquare, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition RSquareDefinition =
        CreateTokenDefinition("param.symbol.square.right", ParamTypes.SymRSquare, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition ColonDefinition =
        CreateTokenDefinition("param.symbol.colon", ParamTypes.SymColon, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition SeparatorDefinition =
        CreateTokenDefinition("param.symbol.separator", ParamTypes.SymSeparator, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition CommaDefinition =
        CreateTokenDefinition("param.symbol.comma", ParamTypes.SymComma, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition AssignDefinition =
        CreateTokenDefinition("param.operator.assign", ParamTypes.OpAssign, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition AddAssignDefinition =
        CreateTokenDefinition("param.operator.assign.add", ParamTypes.OpAddAssign, 1);

    private static readonly IBisLexerOld<ParamTypes>.TokenDefinition SubAssignDefinition =
        CreateTokenDefinition("param.operator.assign.sub", ParamTypes.OpSubAssign, 1);

    private static readonly IEnumerable<IBisLexerOld<ParamTypes>.TokenDefinition> TokenDefinitions = new[]
    {
        ErrorDefinition, LiteralDefinition, PreProcessDefinition, ClassDefinition, EnumDefinition,
        DeleteDefinition, LCurlyDefinition, RCurlyDefinition, EOFDefinition, LSquareDefinition,
        RSquareDefinition, SeparatorDefinition, ColonDefinition, CommaDefinition, AssignDefinition,
        AddAssignDefinition, SubAssignDefinition, IdentifierDefinition, WhitespaceDefinition
    };

    public override IEnumerable<IBisLexerOld<ParamTypes>.TokenDefinition> TokenTypes => TokenDefinitions;
    public override IBisLexerOld<ParamTypes>.TokenDefinition? ErrorToken => ErrorDefinition;
    public override IBisLexerOld<ParamTypes>.TokenDefinition EOFToken => EOFDefinition;
    public bool IsCurrentWhitespace => IsWhitespace();

    public ParamLexerOld(string content) : base(content)
    {
    }

    public IBisLexerOld<ParamTypes>.TokenMatch NextLiteral(params char[] delimiters)
    {
        MoveForward();
        var quoted = false;
        var start = Position;

        if (CurrentChar == '"')
        {
            quoted = true;
            MoveForward();
        }

        while (CurrentChar is { } currentChar && (quoted || !delimiters.Contains(currentChar)))
        {
            switch (currentChar)
            {
                case '\n' or '\r':
                    goto Finish;
                case '"' when quoted:
                {
                    if (MoveForward() != '"')
                    {
                        TraverseWhitespace();
                        if (currentChar != '\\')
                        {
                            goto Finish;
                        }

                        if (MoveForward() != 'n')
                        {
                            goto Finish;
                        }
                        TraverseWhitespace();
                        if (CurrentChar != '"')
                        {
                            goto Finish;
                        }

                        MoveForward();
                    }

                    break;
                }
            }

            MoveForward();
        }

        MoveBackward();
        Finish:
        {
            var match = CreateTokenMatch(start..Position, LiteralDefinition);
            OnTokenMatchedHandler(match, this);
            return match;
        }
    }


    public int SkipWhitespace()
    {
        var i = 0;
        while (NextToken() is { } nextToken &&  nextToken == ParamTypes.AbsWhitespace)
        {
            i++;
        }

        return i;
    }

    protected override IBisLexerOld<ParamTypes>.TokenMatch GetNextToken()
    {
        MoveForward();

        var start = Position;
        if (IsEOF() || CurrentChar is null)
        {
            return CreateTokenMatch(..0, EOFDefinition);
        }

        if (IsWhitespace())
        {
            while (IsWhitespace(PeekForward()))
            {
                MoveForward();
            }

            return CreateTokenMatch(start..Position, WhitespaceDefinition);
        }

        switch (CurrentChar)
        {
            case ',': return CreateTokenMatch(start..Position, CommaDefinition);
            case ';': return CreateTokenMatch(start..Position, SeparatorDefinition);
            case '{': return CreateTokenMatch(start..Position, LCurlyDefinition);
            case '}': return CreateTokenMatch(start..Position, RCurlyDefinition);
            case '[': return CreateTokenMatch(start..Position, LSquareDefinition);
            case ']': return CreateTokenMatch(start..Position, RSquareDefinition);
            case '=': return CreateTokenMatch(start..Position, AssignDefinition);
            case '+' or '-':
            {
                if (PeekForward() != '=')
                {
                    break;
                }

                MoveForward();
                return CreateTokenMatch(start..Position, PreviousChar == '+' ? AddAssignDefinition : SubAssignDefinition);
            }
            case ':': return CreateTokenMatch(start..Position, ColonDefinition);
            case '/':
            {
                switch (PeekForward())
                {
                    case '/':
                    {
                        while (CurrentChar != '\n')
                        {
                            MoveForward();
                        }
                        return CreateTokenMatch(start..Position, PreProcessDefinition);
                    }
                    case '*':
                    {
                        while (!(PreviousChar == '*' && CurrentChar == '/'))
                        {
                            MoveForward();
                        }
                        return CreateTokenMatch(start..Position, PreProcessDefinition);
                    }
                }
                break;
            }
            case '#':
            {
                while (CurrentChar != '\n' && PreviousChar != '\\')
                {
                    MoveForward();
                }
                return CreateTokenMatch(start..Position, PreProcessDefinition);
            }
            case 'c':
            {
                if (PeekForwardMulti(5) == "class")
                {
                    MoveForward(4);
                    return CreateTokenMatch(start..Position, ClassDefinition);
                }
                break;
            }
            case 'd':
            {
                if (PeekForwardMulti(6) == "delete")
                {
                    MoveForward(5);
                    return CreateTokenMatch(start..Position, DeleteDefinition);
                }
                break;
            }
            case 'e':
            {
                if (PeekForwardMulti(4) == "enum")
                {
                    MoveForward(5);
                    return CreateTokenMatch(start..Position, DeleteDefinition);
                }
                break;
            }
        }

        if (IsAlpha())
        {
            while (IsIdentifier(PeekForward()))
            {
                MoveForward();
            }

            return CreateTokenMatch(start..Position, IdentifierDefinition);
        }

        return CreateTokenMatch(start..Position, ErrorDefinition);
    }

    private void TraverseWhitespace()
    {
        while (true)
        {

            switch (CurrentChar)
            {
                case '\r':
                case '\n':
                {
                    MoveForward();
                    break;
                }
                default:
                {
                    if (CurrentChar is { } current)
                    {
                        if (!IsWhitespace(current))
                        {
                            return;
                        }

                        MoveForward();
                    }

                    break;
                }
            }
        }
    }

    private bool IsWhitespace(char? c = null) => (c ?? CurrentChar) switch
    {
        '\t' or '\u000B' or '\u000C' or ' ' or '\r' or '\n' => true,
        _ => false
    };

    private bool IsAlphanum(char? c = null) =>
        char.IsLetterOrDigit(c ?? CurrentChar ?? '\0');

    private bool IsIdentifier(char? c = null)
    {
        var check = c ?? CurrentChar ?? '\0';
        return IsAlphanum(check) || check is '_';
    }

    private bool IsAlpha(char? c = null) =>
        char.IsLetter(c ?? CurrentChar ?? '\0');
}
