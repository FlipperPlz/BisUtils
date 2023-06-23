namespace BisUtils.Param.Lexer;

using Core.Parsing.Lexer;

public class ParamLexer : BisLexer<ParamTypes>
{
    private static readonly IBisLexer<ParamTypes>.TokenDefinition ErrorDefinition = new()
    {
        DebugName = "Error", TokenType = ParamTypes.Invalid, TokenWeight = -1,
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition StringDefinition = new()
    {
        DebugName = "param.StringLiteral", TokenType = ParamTypes.AbsString, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition IntegerDefinition = new()
    {
        DebugName = "param.IntegerLiteral", TokenType = ParamTypes.AbsInteger, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition WhitespaceDefinition = new()
    {
        DebugName = "param.whitespace", TokenType = ParamTypes.AbsWhitespace, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition FloatDefinition = new()
    {
        DebugName = "param.FloatLiteral", TokenType = ParamTypes.AbsFloat, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition PreProcessDefinition = new()
    {
        DebugName = "param.PreProcess", TokenType = ParamTypes.AbsPreprocess, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition IdentifierDefinition = new()
    {
        DebugName = "param.Identifier", TokenType = ParamTypes.AbsIdentifier, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition ClassDefinition = new()
    {
        DebugName = "param.keyword.class", TokenType = ParamTypes.KwClass, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition EnumDefinition = new()
    {
        DebugName = "param.keyword.enum", TokenType = ParamTypes.KwEnum, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition DeleteDefinition = new()
    {
        DebugName = "param.keyword.delete", TokenType = ParamTypes.KwDelete, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition LCurlyDefinition = new()
    {
        DebugName = "param.symbol.curly.left", TokenType = ParamTypes.SymLCurly, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition RCurlyDefinition = new()
    {
        DebugName = "param.symbol.curly.right", TokenType = ParamTypes.SymRCurly, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition LSquareDefinition = new()
    {
        DebugName = "param.symbol.square.left", TokenType = ParamTypes.SymLSquare, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition RSquareDefinition = new()
    {
        DebugName = "param.symbol.square.right", TokenType = ParamTypes.SymRSquare, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition ColonDefinition = new()
    {
        DebugName = "param.symbol.colon", TokenType = ParamTypes.SymColon, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition SeparatorDefinition = new()
    {
        DebugName = "param.symbol.separator", TokenType = ParamTypes.SymSeparator, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition CommaDefinition = new()
    {
        DebugName = "param.symbol.comma", TokenType = ParamTypes.SymComma, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition AssignDefinition = new()
    {
        DebugName = "param.operator.assign", TokenType = ParamTypes.OpAssign, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition AddAssignDefinition = new()
    {
        DebugName = "param.operator.assign.add", TokenType = ParamTypes.OpAddAssign, TokenWeight = 1
    };

    private static readonly IBisLexer<ParamTypes>.TokenDefinition SubAssignDefinition = new()
    {
        DebugName = "param.operator.assign.sub", TokenType = ParamTypes.OpSubAssign, TokenWeight = 1
    };

    private static readonly IEnumerable<IBisLexer<ParamTypes>.TokenDefinition> TokenDefinitions = new[]
    {
        ErrorDefinition, StringDefinition, IntegerDefinition, FloatDefinition, PreProcessDefinition,
        ClassDefinition, EnumDefinition, DeleteDefinition, LCurlyDefinition, RCurlyDefinition,
        LSquareDefinition, RSquareDefinition, SeparatorDefinition, ColonDefinition, CommaDefinition,
        AssignDefinition, AddAssignDefinition, SubAssignDefinition, IdentifierDefinition, WhitespaceDefinition
    };

    private ParamLexerState CurrentLexerState = ParamLexerState.Statement;

    public override IEnumerable<IBisLexer<ParamTypes>.TokenDefinition> TokenTypes => TokenDefinitions;
    public override IBisLexer<ParamTypes>.TokenDefinition ErrorToken => ErrorDefinition;

    public ParamLexer(string content) : base(content)
    {
    }

    protected IBisLexer<ParamTypes>.TokenMatch CreateTokenMatch(Range tokenRange, IBisLexer<ParamTypes>.TokenDefinition tokenDef) =>
        new()
        {
            Success = true,
            TokenLength = tokenRange.End.Value - tokenRange.Start.Value,
            TokenPosition = tokenRange.Start.Value,
            TokenText = GetRange(tokenRange),
            TokenType = tokenDef
        };

    protected override IBisLexer<ParamTypes>.TokenMatch GetNextToken()
    {
        var start = Position;

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
            case ';': return CreateTokenMatch(start..Position, SeparatorDefinition);
            case '[': return CreateTokenMatch(start..Position, LSquareDefinition);
            case ']': return CreateTokenMatch(start..Position, RSquareDefinition);
            case '=': return CreateTokenMatch(start..Position, AssignDefinition);
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

        }

        return CreateTokenMatch(start..Position, ErrorToken);
    }

    private bool IsWhitespace(char? c = null) => (c ?? CurrentChar) switch
    {
        '\t' or '\u000B' or '\u000C' or ' ' or '\r' or '\n' => true,
        _ => false
    };

    private bool IsAlphanum(char? c = null) =>
        char.IsLetterOrDigit(c ?? CurrentChar ?? '\0');
}
