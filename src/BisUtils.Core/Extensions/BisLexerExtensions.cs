namespace BisUtils.Core.Extensions;

using Parsing.Lexer;
using Parsing.Token;
using Parsing.Token.Matching;
using Parsing.Token.Typing;

public static class BisLexerExtensions
{

    public static void TokenizeUntilEnd<TTokenSet>(this IBisLexer<TTokenSet> lexer) where TTokenSet : IBisTokenTypeSet
    {
        while (!lexer.IsEOF())
        {
            lexer.LexToken();
        }
    }

    public static bool IfMatches(this IBisTokenMatch match,IBisTokenType validTokenType, Action<IBisTokenMatch> predicate)
    {
        if (validTokenType == match.TokenType)
        {
            return false;
        }

        predicate(match);
        return true;


    }
}
