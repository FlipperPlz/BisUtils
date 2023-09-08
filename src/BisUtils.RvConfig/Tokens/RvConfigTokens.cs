namespace BisUtils.RvConfig.Tokens;

using LangAssembler.Lexer.Base;
using LangAssembler.Lexer.Models.Match;
using LangAssembler.Lexer.Models.Type;
using RvLex.Tokens;

public interface IRvConfigTokenSet : IRvTokenSet
{
    public static abstract ITokenType RvRightCurly { get; }
    TokenMatcher RightCurlyMatcher => MatchRightCurly;
    static bool MatchRightCurly(ILexer lexer, long tokenStart, int? currentChar)
        => currentChar == '}';

    public static abstract ITokenType RvLeftCurly { get; }
    TokenMatcher LeftCurlyMatcher => MatchLeftCurly;
    static bool MatchLeftCurly(ILexer lexer, long tokenStart, int? currentChar)
        => currentChar == '{';

    public static abstract ITokenType RvRightSquare { get; }
    TokenMatcher RightSquareMatcher => MatchRightSquare;
    static bool MatchRightSquare(ILexer lexer, long tokenStart, int? currentChar)
        => currentChar == ']';

    public static abstract ITokenType RvLeftSquare { get; }
    TokenMatcher LeftSquareMatcher => MatchLeftSquare;
    static bool MatchLeftSquare(ILexer lexer, long tokenStart, int? currentChar)
        => currentChar == '[';

    public static abstract ITokenType RvColon { get; }
    TokenMatcher ColonMatcher => MatchColon;
    static bool MatchColon(ILexer lexer, long tokenStart, int? currentChar)
        => currentChar == ':';

    public static abstract ITokenType RvSemicolon { get; }
    TokenMatcher SemicolonMatcher => MatchSemicolon;
    static bool MatchSemicolon(ILexer lexer, long tokenStart, int? currentChar)
        => currentChar == ';';


    public static abstract ITokenType RvAssign { get; }
    TokenMatcher AssignMatcher => MatchAssign;
    static bool MatchAssign(ILexer lexer, long tokenStart, int? currentChar)
        => currentChar == '=';
}


public sealed class RvConfigTokenSet : RvTokenSet, IRvConfigTokenSet
{
    public static new readonly IRvConfigTokenSet Instance = new RvConfigTokenSet();
    public static ITokenType RvRightCurly =>
        new TokenType("param.symbol.curly.right", Instance.RightCurlyMatcher);
    public static ITokenType RvLeftCurly =>
        new TokenType("param.symbol.curly.left", Instance.LeftCurlyMatcher);
    public static ITokenType RvRightSquare =>
        new TokenType("param.symbol.square.left", Instance.LeftSquareMatcher);
    public static ITokenType RvLeftSquare =>
        new TokenType("param.symbol.square.right", Instance.RightSquareMatcher);
    public static ITokenType RvColon =>
        new TokenType("param.symbol.colon", Instance.ColonMatcher);
    public static ITokenType RvSemicolon =>
        new TokenType("param.symbol.semicolon", Instance.ColonMatcher);
    public static ITokenType RvAssign =>
        new TokenType("param.symbol.assign", Instance.AssignMatcher);

    protected override void InitializeTypes()
    {
        InitializeType(RvRightCurly);
        InitializeType(RvLeftCurly);
        InitializeType(RvRightSquare);
        InitializeType(RvLeftSquare);
        InitializeType(RvColon);
        InitializeType(RvSemicolon);
        base.InitializeTypes();

    }
    //
    // public static readonly IBisTokenType ConfigLiteral =
    //     new BisTokenType("param.abstract.literal", ToComplex);
    //
    // public static readonly IBisTokenType ConfigSubAssign =
    //     new BisTokenType("param.symbol.assign.subtract", "-=");
    //
    // public static readonly IBisTokenType ConfigAddAssign =
    //     new BisTokenType("param.symbol.assign.add", "+=");
    //
    // public static readonly IBisTokenType ConfigClass =
    //     new BisTokenType("param.keyword.class", "class");
    //
    // public static readonly IBisTokenType ConfigQuoteEscape =
    //     new BisTokenType("param.string.escape.quote", "\"\"");
    //
    // public static readonly IBisTokenType ConfigDelete =
    //     new BisTokenType("param.keyword.delete", "delete");
    //
    // public static readonly IBisTokenType ConfigEnum =
    //     new BisTokenType("param.keyword.enum", "enum");
}

