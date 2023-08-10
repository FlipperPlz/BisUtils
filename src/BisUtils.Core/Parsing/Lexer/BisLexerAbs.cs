namespace BisUtils.Core.Parsing.Lexer;

using Singleton;
using Token.Tokens;
using Token.Typing;

public interface IBisLexerAbs<out TTokens> : IBisLexer<TTokens> where TTokens : BisTokenTypeSet<TTokens>, new()
{
}

public abstract class BisLexerAbs<TTokens> : BisLexerCore, IBisLexerAbs<TTokens> where TTokens : BisTokenTypeSet<TTokens>, new()
{
    private readonly bool parseBaseFirst;
    public TTokens LexicalTokenSet => BisSingletonProvider.LocateInstance<TTokens>();

    protected BisLexerAbs(string content, bool parseBaseFirst) : base(content) => this.parseBaseFirst = parseBaseFirst;

    protected sealed override IBisTokenType FindNextMatch(int tokenStart, char? currentChar)
    {
        var match = parseBaseFirst ? base.FindNextMatch(tokenStart, currentChar) : LocateExtendedMatch(tokenStart, currentChar);
        if (match is not BisInvalidTokeType)
        {
            return match;
        }

        return !parseBaseFirst ? base.FindNextMatch(tokenStart, currentChar) : match;
    }

    protected abstract IBisTokenType LocateExtendedMatch(int tokenStart, char? currentChar);

    protected abstract override IBisTokenType LocateNextMatch(int tokenStart, char? currentChar);
}
