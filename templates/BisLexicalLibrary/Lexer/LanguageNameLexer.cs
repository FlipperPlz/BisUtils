using BisLexicalLibrary.Tokens;

namespace $name$.LOL;

using BisUtils.Core.Parsing.Lexer;
using BisUtils.Core.Parsing.Token.Typing;
using Tokens;

public interface ILanguageNameLexer<out TTokens> : IBisLexer<TTokens> where TTokens : LanguageNameTokens<TTokens>
{

    public IBisTokenType TryMatchDickButt();
}

public class EnfusionLexer<TTokens> : BisLexer<TTokens>, ILanguageNameLexer<TTokens>
    where TTokens : LanguageNameTokens<TTokens>, new()
{
    public EnfusionLexer(string content) : base(content)
    {
    }

    protected override IBisTokenType? LocateNextMatch(int tokenStart)
        => TryMatchDickButt();

    public IBisTokenType TryMatchDickButt()
        => throw new NotImplementedException();
}
