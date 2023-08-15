namespace BisUtils.RvConfig.Lexer;

using Core.Parsing.Token.Tokens;
using Core.Parsing.Token.Typing;
using Enumerations;
using RvLex.Lexer;
using Tokens;

public interface IRvConfigLexer : IRvLexer<RvConfigTokenSet>
{
    public IBisTokenType MatchCurly(bool isLeft);
    public IBisTokenType MatchSquare(bool isLeft);
    public IBisTokenType MatchColon();
    public IBisTokenType MatchQuotedString(int tokenStart, char? currentChar);
    public IBisTokenType TryMatchOperator(ParamOperatorType type);
    public IBisTokenType TryMatchSubAssign();
    public IBisTokenType TryMatchAddAssign();
    public IBisTokenType TryMatchKeyword(RvConfigKeywordType type);
    public IBisTokenType TryMatchIdentifier();

    public IBisTokenType MatchAssignOperator();
    public IBisTokenType MatchSeparator();


    public static bool IsIdentifierChar(char? c, bool isFirst = false)
    {
        if (c is not { } ch)
        {
            return false;
        }

        if (char.IsLetter(ch) || ch is '_')
        {
            return true;
        }

        return char.IsAsciiDigit(ch) && isFirst;
    }
}

public sealed class RvConfigLexer : RvLexer<RvConfigTokenSet>, IRvConfigLexer
{
    public RvConfigLexer(string content) : base(content)
    {
    }


    protected override IBisTokenType LocateExtendedMatch(int tokenStart, char? currentChar) => currentChar switch
    {
        '{' => MatchCurly(true),
        '}' => MatchCurly(false),
        ';' => MatchSeparator(),
        ':' => MatchColon(),
        '[' => MatchSquare(true),
        ']' => MatchSquare(false),
        '+' => TryMatchOperator(ParamOperatorType.AddAssign),
        '-' => TryMatchOperator(ParamOperatorType.SubAssign),
        '=' => TryMatchOperator(ParamOperatorType.Assign),
        'c' => TryMatchKeyword(RvConfigKeywordType.Class),
        'e' => TryMatchKeyword(RvConfigKeywordType.Enum),
        'd' => TryMatchKeyword(RvConfigKeywordType.Delete),
        _ => TryMatchIdentifier()
    };

    public override IBisTokenType MatchQuote(int tokenStart, char? currentChar) =>
        PeekForward() == '"' ? RvConfigTokenSet.ConfigQuoteEscape : MatchQuotedString(tokenStart, currentChar);

    public IBisTokenType MatchQuotedString(int tokenStart, char? currentChar)
    {
        throw new NotImplementedException();
    }


    public IBisTokenType TryMatchIdentifier()
    {
        if (!IRvConfigLexer.IsIdentifierChar(CurrentChar, true))
        {
            return InvalidToken;
        }

        while (IRvConfigLexer.IsIdentifierChar(PeekForward()))
        {
            MoveForward();
        }

        return RvConfigTokenSet.RvIdentifier;
    }

    public IBisTokenType TryMatchOperator(ParamOperatorType type) => type switch
    {
        ParamOperatorType.Assign => MatchAssignOperator(),
        ParamOperatorType.AddAssign => TryMatchAddAssign(),
        ParamOperatorType.SubAssign => TryMatchSubAssign(),
        _ => InvalidToken
    };

    public IBisTokenType TryMatchSubAssign()
    {
        if (CurrentChar != '-' || MoveForward() != '=')
        {
            return InvalidToken;
        }

        return RvConfigTokenSet.ConfigSubAssign;
    }

    public IBisTokenType TryMatchAddAssign()
    {
        if (CurrentChar != '+' || MoveForward() != '=')
        {
            return InvalidToken;
        }

        return RvConfigTokenSet.ConfigAddAssign;
    }

    public IBisTokenType TryMatchKeyword(RvConfigKeywordType type)
    {
        var keyWordType = type switch
        {
            RvConfigKeywordType.Class => TryMatchWord("class", RvConfigTokenSet.ConfigClass),
            RvConfigKeywordType.Enum => TryMatchWord("enum", RvConfigTokenSet.ConfigEnum),
            RvConfigKeywordType.Delete => TryMatchWord("delete", RvConfigTokenSet.ConfigDelete),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        return keyWordType is BisInvalidTokeType ? TryMatchIdentifier() : keyWordType;
    }

    public IBisTokenType MatchAssignOperator() =>
        RvConfigTokenSet.ConfigAssign;

    public IBisTokenType MatchSquare(bool isLeft) =>
        isLeft ? RvConfigTokenSet.ConfigLSquare : RvConfigTokenSet.ConfigRSquare;

    public IBisTokenType MatchColon() =>
        RvConfigTokenSet.ConfigColon;

    public IBisTokenType MatchCurly(bool isLeft) =>
        isLeft ? RvConfigTokenSet.ConfigLCurly : RvConfigTokenSet.ConfigRCurly;

    public IBisTokenType MatchSeparator() => RvConfigTokenSet.ConfigSeparator;



}
