namespace BisUtils.RvLex.Lexer;

using Core.Parsing.Lexer;
using Core.Parsing.Token.Typing;
using Tokens;

public interface IRvLexer<out TTokens> : IBisLexerAbs<TTokens> where TTokens : RvTokenSet<TTokens>, new()
{
    public IBisTokenType TryMatchNewLine();
    public IBisTokenType TryMatchComma();
}

public abstract class RvLexer<TTokens> : BisLexerAbs<TTokens>, IRvLexer<TTokens>
    where TTokens : RvTokenSet<TTokens>, new()
{
    protected RvLexer(string content) : base(content, true)
    {
    }

    protected abstract override IBisTokenType LocateExtendedMatch(int tokenStart, char? currentChar);

    protected override IBisTokenType LocateNextMatch(int tokenStart, char? currentChar) =>currentChar switch
    {
        '\n' => TryMatchNewLine(),
        ',' => TryMatchComma(),
        _ => InvalidToken
    };

    public IBisTokenType TryMatchNewLine() => TryMatchChar('\n', RvTokenSet.RvNewLine);

    public IBisTokenType TryMatchComma() => TryMatchChar(',', RvTokenSet.RvComma);
}
