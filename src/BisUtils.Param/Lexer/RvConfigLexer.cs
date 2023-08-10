namespace BisUtils.Param.Lexer;

using Core.Parsing.Token.Tokens;
using Core.Parsing.Token.Typing;
using Enumerations;
using RvLex.Lexer;
using Tokens;

public interface IRvConfigLexer : IRvLexer<RvConfigTokenSet>
{
    public IBisTokenType MatchWhitespace();
    public IBisTokenType MatchCurly(bool isLeft);
    public IBisTokenType MatchSquare(bool isLeft);
    public IBisTokenType MatchColon();
    public IBisTokenType TryMatchOperator(ParamOperatorType assignmentType);
    public IBisTokenType TryMatchSubAssign();
    public IBisTokenType TryMatchAddAssign();


    public IBisTokenType MatchAssignOperator();
    public IBisTokenType MatchSeparator();

    public static bool IsWhitespace(char? c) => c switch
    {
        '\t' or '\u000B' or '\u000C' or ' ' or '\r' or '\n' => true,
        _ => false
    };
}

public sealed class RvConfigLexer : RvLexer<RvConfigTokenSet>, IRvConfigLexer
{
    public RvConfigLexer(string content) : base(content)
    {
    }


    protected override IBisTokenType LocateExtendedMatch(int tokenStart, char? currentChar)
    {
        if (IRvConfigLexer.IsWhitespace(currentChar))
        {
            return MatchWhitespace();
        }

        switch (currentChar)
        {
            case '{': return MatchCurly(true);
            case '}': return MatchCurly(false);
            case ';': return MatchSeparator();
            case ':': return MatchColon();
            case '[': return MatchSquare(true);
            case ']': return MatchSquare(false);
            case '+': return TryMatchOperator(ParamOperatorType.AddAssign);
            case '-': return TryMatchOperator(ParamOperatorType.SubAssign);
            case '=': return TryMatchOperator(ParamOperatorType.Assign);

        }

        return InvalidToken;
    }

    public IBisTokenType TryMatchOperator(ParamOperatorType assignmentType) => assignmentType switch
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

    public IBisTokenType MatchAssignOperator() =>
        RvConfigTokenSet.ConfigAssign;

    public IBisTokenType MatchSquare(bool isLeft) =>
        isLeft ? RvConfigTokenSet.ConfigLSquare : RvConfigTokenSet.ConfigRSquare;

    public IBisTokenType MatchColon() =>
        RvConfigTokenSet.ConfigColon;

    public IBisTokenType MatchCurly(bool isLeft) =>
        isLeft ? RvConfigTokenSet.ConfigLCurly : RvConfigTokenSet.ConfigRCurly;

    public IBisTokenType MatchSeparator() => RvConfigTokenSet.ConfigSeparator;


    public IBisTokenType MatchWhitespace() =>
        MatchWhile(IRvConfigLexer.IsWhitespace, RvConfigTokenSet.ConfigWhitespace, false);

}
