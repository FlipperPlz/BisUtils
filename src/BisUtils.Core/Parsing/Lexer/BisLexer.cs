namespace BisUtils.Core.Parsing.Lexer;

public interface IBisLexer<TTokenEnum> where TTokenEnum : Enum
{
    public readonly struct TokenMatch {

        public override bool Equals(object? obj) => obj is TokenMatch other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(TokenType, TokenPosition, TokenLength, TokenText, Success);

        public required TokenDefinition TokenType { get; init; }
        public required long TokenPosition { get; init; }
        public required long TokenLength { get; init; }
        public required string TokenText { get; init; }
        public required bool Success { get; init; }

        public bool Equals(TokenMatch other) =>
            TokenType.Equals(other.TokenType) && TokenPosition == other.TokenPosition && TokenLength == other.TokenLength && TokenText == other.TokenText && Success == other.Success;

        public static bool operator ==(TokenMatch left, TTokenEnum? right) =>
            EqualityComparer<TTokenEnum>.Default.Equals(left.TokenType.TokenId, right);

        public static bool operator !=(TokenMatch left, TTokenEnum? right) =>
            !(left == right);
    }

    public readonly struct TokenDefinition
    {
        public bool Equals(TokenDefinition other) =>
            DebugName == other.DebugName && EqualityComparer<TTokenEnum>.Default.Equals(TokenId, other.TokenId) && TokenWeight == other.TokenWeight;

        public override bool Equals(object? obj) => obj is TokenDefinition other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(DebugName, TokenId, TokenWeight);

        public required string? DebugName { get; init; }
        public required TTokenEnum TokenId { get; init; }
        public required short TokenWeight { get; init; }

        public static bool operator ==(TokenDefinition left, TTokenEnum? right) =>
            EqualityComparer<TTokenEnum>.Default.Equals(left.TokenId, right);

        public static bool operator !=(TokenDefinition left, TTokenEnum? right) =>
            !(left == right);
    }

    public delegate void TokenMatched(TokenMatch match, IBisLexer<TTokenEnum> lexer);
    public event TokenMatched? OnTokenMatched;

    protected IEnumerable<TokenDefinition> TokenTypes { get; }
    protected List<TokenMatch> PreviousMatches { get; }

    protected TokenDefinition? ErrorToken { get; }
    protected TokenDefinition EOFToken { get; }


    public TokenMatch? PreviousMatch();
    public TokenMatch NextToken();

    public void TokenizeUntilEnd();
}

public abstract class BisLexer<TTokenEnum> : BisMutableStringStepper, IBisLexer<TTokenEnum> where TTokenEnum : Enum
{
    public event IBisLexer<TTokenEnum>.TokenMatched? OnTokenMatched;
    public abstract IEnumerable<IBisLexer<TTokenEnum>.TokenDefinition> TokenTypes { get; }
    public abstract IBisLexer<TTokenEnum>.TokenDefinition? ErrorToken { get; }
    public abstract IBisLexer<TTokenEnum>.TokenDefinition EOFToken { get; }
    public List<IBisLexer<TTokenEnum>.TokenMatch> PreviousMatches { get; } = new();

    protected BisLexer(string content) : base(content)
    {
    }

    public IBisLexer<TTokenEnum>.TokenMatch? PreviousMatch() => PreviousMatches.LastOrDefault();

    public IBisLexer<TTokenEnum>.TokenMatch NextToken()
    {
        var value = GetNextToken();
        OnTokenMatchedHandler(value, this);
        return value;
    }

    public void TokenizeUntilEnd()
    {
        while (NextToken() != EOFToken.TokenId)
        {
        }
    }

    protected virtual void OnTokenMatchedHandler(IBisLexer<TTokenEnum>.TokenMatch match, IBisLexer<TTokenEnum> lexer)
    {
        PreviousMatches.Add(match);
        OnTokenMatched?.Invoke(match, lexer);
    }

    protected static IBisLexer<TTokenEnum>.TokenDefinition CreateTokenDefinition(string debugName, TTokenEnum tokenType, short tokenWeight) =>
        new() { DebugName = debugName, TokenId = tokenType, TokenWeight = tokenWeight };

    protected IBisLexer<TTokenEnum>.TokenMatch CreateTokenMatch(Range tokenRange, IBisLexer<TTokenEnum>.TokenDefinition tokenDef) =>
        new() {
            Success = true,
            TokenLength = tokenRange.End.Value - tokenRange.Start.Value + 1,
            TokenPosition = tokenRange.Start.Value,
            TokenText = GetRange(tokenRange.Start.Value..(tokenRange.End.Value + 1)),
            TokenType = tokenDef
        };

    protected static IBisLexer<TTokenEnum>.TokenMatch CreateTokenMatch(Range tokenRange, string str, IBisLexer<TTokenEnum>.TokenDefinition tokenDef) =>
        new() {
            Success = true,
            TokenLength = tokenRange.End.Value - tokenRange.Start.Value + 1,
            TokenPosition = tokenRange.Start.Value,
            TokenText = str,
            TokenType = tokenDef
        };


    protected abstract IBisLexer<TTokenEnum>.TokenMatch GetNextToken();

}
