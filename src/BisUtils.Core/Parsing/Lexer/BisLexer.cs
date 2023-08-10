namespace BisUtils.Core.Parsing.Lexer;

using Token.Matching;
using Token.Typing;

public interface IBisLexer : IBisMutableStringStepper
{
    public event EventHandler<BisTokenMatch> OnTokenMatched;

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

public abstract class BisLexer<TTokens> : BisLexerCore, IBisLexer<TTokens> where TTokens : BisTokenTypeSet<TTokens>, new()
{
    public TTokens LexicalTokenSet => BisTokenExtensions.FindTokenSet<TTokens>();

    protected BisLexer(string content) : base(content)
    {
    }

    protected abstract override IBisTokenType LocateNextMatch(int tokenStart, char? currentChar);
}

