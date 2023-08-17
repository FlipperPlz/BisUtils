namespace BisUtils.Core.ParsingFramework.Lexer;

using Steppers.Mutable;
using Tokens.Match;
using Tokens.TypeSet;

public interface IBisLexer : IBisMutableStringStepper
{
    public event BisMatchHandler OnTokenMatched;

    public bool EventsMuted { get; }
    public BisTokenMatch? LastMatchedToken { get; }
    public IEnumerable<BisTokenMatch> PreviousMatches { get; }
    public int LineNumber { get; }
    public int ColumnNumber { get; }
    public int LineStart { get; }
    public BisTokenMatch? MatchForIndex(int tokenIndex);
    public BisTokenMatch LexToken();
}

public interface IBisLexer<out TTokens> : IBisLexer where TTokens : IBisTokenTypeSet
{
    public TTokens LexicalTokenSet { get; }
}
