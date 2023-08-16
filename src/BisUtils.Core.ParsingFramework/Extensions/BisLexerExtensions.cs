namespace BisUtils.Core.ParsingFramework.Extensions;

using Lexer;
using Tokens.Match;
using Tokens.Type;
using Tokens.TypeSet;

public static class BisLexerExtensions
{
    public static void TokenizeUntilEnd<TTokenSet>(this IBisLexer<TTokenSet> lexer) where TTokenSet : IBisTokenTypeSet
    {
        while (!lexer.IsEOF())
        {
            lexer.LexToken();
        }
    }

    public static bool IfMatches(this IBisTokenMatch match, IBisTokenType validTokenType, Action<IBisTokenMatch> predicate)
    {
        if (validTokenType != match.TokenType)
        {
            return false;
        }

        predicate(match);
        return true;

    }

    public static bool IsNotType(this IBisTokenMatch match, IBisTokenType type) => !IsType(match, type);


    public static bool IsType(this IBisTokenMatch match, IBisTokenType type) => match.TokenType == type;

    public static bool IfDoesntMatch(this IBisTokenMatch match, IBisTokenType invalidTokenType, Action<IBisTokenMatch> predicate)
    {
        if (invalidTokenType == match.TokenType)
        {
            return false;
        }

        predicate(match);
        return true;
    }
}
