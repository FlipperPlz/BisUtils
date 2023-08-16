namespace BisUtils.Core.ParsingFramework.Lexer;

using Singleton;
using Tokens.Type;
using Tokens.Type.Types;
using Tokens.TypeSet;

public abstract class BisLexerAbs<TTokens> : BisLexerCore, IBisLexerAbs<TTokens> where TTokens : BisTokenTypeSet<TTokens>, new()
{
    private readonly bool parseBaseFirst;
    public TTokens LexicalTokenSet => BisSingletonProvider.LocateInstance<TTokens>();

    protected BisLexerAbs(string content, bool parseBaseFirst) : base(content) => this.parseBaseFirst = parseBaseFirst;

    protected sealed override IBisTokenType FindNextMatch(int tokenStart, char? currentChar)
    {
        var match = parseBaseFirst ? base.FindNextMatch(tokenStart, currentChar) : LocateExtendedMatch(tokenStart, currentChar);
        if (match is not BisInvalidTokenType)
        {
            return match;
        }

        return !parseBaseFirst ? base.FindNextMatch(tokenStart, currentChar) : match;
    }

    protected abstract IBisTokenType LocateExtendedMatch(int tokenStart, char? currentChar);

    protected abstract override IBisTokenType LocateNextMatch(int tokenStart, char? currentChar);
}
