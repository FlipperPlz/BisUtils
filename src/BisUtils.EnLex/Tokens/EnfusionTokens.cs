namespace BisUtils.EnLex.Tokens;

using LangAssembler.Lexer.Base;
using LangAssembler.Lexer.Models.Match;
using LangAssembler.Lexer.Models.Type;
using LangAssembler.Lexer.Models.TypeSet;

// ReSharper disable file StaticMemberInGenericType

public interface IEnfusionTokenSet : ITokenTypeSet
{
    public static abstract ITokenType EnfusionNewLine { get; }
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

    public static abstract ITokenType EnfusionWhitespace { get; }
    TokenMatcher WhitespaceMatcher => MatchWhitespace;
    static bool MatchWhitespace(ILexer lexer, long tokenStart, int? currentChar)
    {
        //TODO:
        return false;
    }


    public static abstract ITokenType EnfusionIdentifier { get; }
    TokenMatcher IdentifierMatcher => MatchIdentifier;
    static bool MatchIdentifier(ILexer lexer, long tokenStart, int? currentChar)
    {

        //TODO:
        return false;
    }


}

public class EnfusionTokenSet : TokenTypeSet, IEnfusionTokenSet
{

    public static readonly IEnfusionTokenSet Instance =
        new EnfusionTokenSet();
    public static ITokenType EnfusionNewLine =>
        new TokenType("enfusion.newline", Instance.NewLineMatcher);
    public static ITokenType EnfusionWhitespace =>
        new TokenType("enfusion.abstract.whitespace", Instance.WhitespaceMatcher);
    public static ITokenType EnfusionIdentifier =>
        new TokenType("enfusion.identifier", Instance.IdentifierMatcher);


    protected EnfusionTokenSet()
    {

    }

    protected override void InitializeTypes()
    {
        InitializeType(EnfusionNewLine);
        InitializeType(EnfusionWhitespace);
        InitializeType(EnfusionIdentifier);
    }
}
