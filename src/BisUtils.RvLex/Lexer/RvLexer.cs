namespace BisUtils.RvLex.Lexer;

using Core.Parsing.Lexer;
using Core.Parsing.Token.Matching;
using Core.Parsing.Token.Typing;
using Tokens;

public interface IRvLexer<out TTokens> : IBisLexerAbs<TTokens> where TTokens : RvTokenSet<TTokens>, new()
{
    IBisTokenType MatchWhitespace();
    IBisTokenType MatchQuote(int tokenStart, char? currentChar);
    IBisTokenType MatchNewLine(int tokenStart, char? currentChar);
    IBisTokenType MatchComma(int tokenStart, char? currentChar);
    int LexWhitespace(ref BisTokenMatch match, bool matchCurrent = false);



    bool IsWhitespace(char? c);
}

public abstract class RvLexer<TTokens> : BisLexerAbs<TTokens>, IRvLexer<TTokens>
    where TTokens : RvTokenSet<TTokens>, new()
{
    protected RvLexer(string content) : base(content, true)
    {
    }

    protected abstract override IBisTokenType LocateExtendedMatch(int tokenStart, char? currentChar);

    protected override IBisTokenType LocateNextMatch(int tokenStart, char? currentChar)
    {

        if (IsWhitespace(currentChar))
        {
            return MatchWhitespace();
        }

        return currentChar switch
        {
            '\n' => MatchNewLine(tokenStart, currentChar),
            ',' => MatchNewLine(tokenStart, currentChar),
            '"' => MatchQuote(tokenStart, currentChar),
            _ => InvalidToken
        };
    }


    public virtual int LexWhitespace(ref BisTokenMatch match, bool matchCurrent = false)
    {

        if (!matchCurrent)
        {
            match = LexToken();
        }
        var i = 0;
        while (match.TokenType == RvTokenSet.RvWhitespace || match.TokenType == RvTokenSet.RvNewLine)
        {
            match = LexToken();
            i++;
        }

        return i;
    }

    public virtual IBisTokenType MatchComma(int tokenStart, char? currentChar) => RvTokenSet.RvComma;

    public virtual bool IsWhitespace(char? c) => (c ?? CurrentChar) switch
    {
        '\t' or '\u000B' or '\u000C' or ' ' or '\r' or '\n' => true,
        _ => false
    };

    public virtual IBisTokenType MatchWhitespace() =>
        MatchWhile(IsWhitespace, RvTokenSet.RvWhitespace, false);

    public virtual IBisTokenType MatchNewLine(int tokenStart, char? currentChar) => RvTokenSet.RvNewLine;

    public virtual IBisTokenType MatchQuote(int tokenStart, char? currentChar) => RvTokenSet.RvQuote;
}
