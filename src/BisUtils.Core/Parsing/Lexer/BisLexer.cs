namespace BisUtils.Core.Parsing.Lexer;

public interface IBisLexer<TTokenEnum>
{
    public readonly struct TokenMatch {
        public TokenDefinition TokenType { get; init; }
        public long TokenPosition { get; init; }
        public long TokenLength { get; init; }
        public string TokenText { get; init; }
        public bool Success { get; init; }
    }

    public readonly struct TokenDefinition
    {
        public string? DebugName { get; init; }
        public TTokenEnum TokenType { get; init; }
        public short TokenWeight { get; init; }
    }


    public event TokenMatched? OnTokenMatched;

    public delegate void TokenMatched(TokenMatch match, IBisLexer<TTokenEnum> lexer);
    protected Span<TokenDefinition> TokenTypes { get; }
    protected TokenDefinition ErrorToken { get; }
    protected TokenMatch? PreviousMatch { get; }

    public TokenMatch NextToken();

}

public abstract class BisLexer<TTokenEnum> : BisMutableStringStepper, IBisLexer<TTokenEnum>
{
    public event IBisLexer<TTokenEnum>.TokenMatched? OnTokenMatched;
    public abstract Span<IBisLexer<TTokenEnum>.TokenDefinition> TokenTypes { get; }
    public abstract IBisLexer<TTokenEnum>.TokenDefinition ErrorToken { get; }
    public IBisLexer<TTokenEnum>.TokenMatch? PreviousMatch { get; protected set; }

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
        PreviousMatch = match;
        OnTokenMatched?.Invoke(match, lexer);
    }

    protected abstract IBisLexer<TTokenEnum>.TokenMatch GetNextToken();

}
