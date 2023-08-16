namespace BisUtils.Core.ParsingFramework.Lexer;

using Extensions;
using Tokens.Type;
using Tokens.TypeSet;

public abstract class BisLexer<TTokens> : BisLexerCore, IBisLexer<TTokens> where TTokens : BisTokenTypeSet<TTokens>, new()
{
    public TTokens LexicalTokenSet => BisTokenExtensions.FindTokenSet<TTokens>();

    protected BisLexer(string content) : base(content)
    {
    }

    protected abstract override IBisTokenType LocateNextMatch(int tokenStart, char? currentChar);
}
