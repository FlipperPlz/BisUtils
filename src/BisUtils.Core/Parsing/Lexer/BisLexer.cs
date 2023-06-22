namespace BisUtils.Core.Parsing.Lexer;

public interface IBisLexer<TTokenEnum>
{
    public readonly struct TokenMatch {
        public required TokenDefinition TokenType { get; init; }
        public required long TokenPosition { get; init; }
        public required long TokenLength { get; init; }
        public required string TokenText { get; init; }
        public required bool Success { get; init; }
    }

    public readonly struct TokenDefinition
    {
        public required string? DebugName { get; init; }
        public required TTokenEnum TokenType { get; init; }
        public required short TokenWeight { get; init; }
    }

    public delegate void TokenMatched(TokenMatch match, IBisLexer<TTokenEnum> lexer);
    public event TokenMatched? OnTokenMatched;

    protected IEnumerable<TokenDefinition> TokenTypes { get; }
    protected List<TokenMatch> PreviousMatches { get; }

    protected TokenDefinition ErrorToken { get; }

    public TokenMatch NextToken();

}

public abstract class BisLexer<TTokenEnum> : BisMutableStringStepper, IBisLexer<TTokenEnum>
{
    public event IBisLexer<TTokenEnum>.TokenMatched? OnTokenMatched;
    public abstract IEnumerable<IBisLexer<TTokenEnum>.TokenDefinition> TokenTypes { get; }
    public abstract IBisLexer<TTokenEnum>.TokenDefinition ErrorToken { get; }
    public List<IBisLexer<TTokenEnum>.TokenMatch> PreviousMatches { get; } = new();

    protected BisLexer(string content) : base(content)
    {
    }

    public IBisLexer<TTokenEnum>.TokenMatch NextToken()
    {
        var value = GetNextToken();
        OnTokenMatchedHandler(value, this);
        return value;
    }

    protected virtual void OnTokenMatchedHandler(IBisLexer<TTokenEnum>.TokenMatch match, IBisLexer<TTokenEnum> lexer)
    {
        PreviousMatches.Add(match);
        OnTokenMatched?.Invoke(match, lexer);
    }

    protected abstract IBisLexer<TTokenEnum>.TokenMatch GetNextToken();

}
