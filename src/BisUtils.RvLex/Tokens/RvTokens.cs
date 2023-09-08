namespace BisUtils.RvLex.Tokens;

using LangAssembler.Lexer.Base;
using LangAssembler.Lexer.Models.Match;
using LangAssembler.Lexer.Models.Type;
using LangAssembler.Lexer.Models.TypeSet;

// ReSharper disable file StaticMemberInGenericType

public interface IRvTokenSet : ITokenTypeSet
{
    public static abstract ITokenType RvNewLine { get; }
    TokenMatcher NewLineMatcher => MatchNewLine;

    static bool MatchNewLine(ILexer lexer, long tokenStart, int? currentChar)
    {
        switch (currentChar)
        {
            case '\n': break;
            case '\r':
            {
                if (lexer.PeekNext() == '\n')
                {
                    lexer.MoveForward();
                }

                break;
            }
            default:
                return false;
        }

        return true;
    }

    public static abstract ITokenType RvWhitespace { get; }
    TokenMatcher WhitespaceMatcher => MatchWhitespace;
    static bool MatchWhitespace(ILexer lexer, long tokenStart, int? currentChar)
    {
        //TODO:
        return false;
    }


    public static abstract ITokenType RvIdentifier { get; }
    TokenMatcher IdentifierMatcher => MatchIdentifier;
    static bool MatchIdentifier(ILexer lexer, long tokenStart, int? currentChar)
    {

        //TODO:
        return false;
    }


}

public class RvTokenSet : TokenTypeSet, IRvTokenSet
{

    public static readonly IRvTokenSet Instance =
        new RvTokenSet();
    public static ITokenType RvNewLine =>
        new TokenType("rv.newline", Instance.NewLineMatcher);
    public static ITokenType RvWhitespace =>
        new TokenType("rv.abstract.whitespace", Instance.WhitespaceMatcher);
    public static ITokenType RvIdentifier =>
        new TokenType("rv.identifier", Instance.IdentifierMatcher);


    protected RvTokenSet()
    {

    }

    protected override void InitializeTypes()
    {
        InitializeType(RvNewLine);
        InitializeType(RvWhitespace);
        InitializeType(RvIdentifier);
    }
}
