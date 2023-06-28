namespace BisUtils.Core.Parsing.Lexer;

using System.Text;
using FResults;

public interface IBisLexer<TTokenEnum> where TTokenEnum : Enum
{
    /// <summary>
    /// Represents a token match instance.
    /// </summary>
    public readonly struct TokenMatch {

        public override bool Equals(object? obj) => obj is TokenMatch other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(TokenType, TokenPosition, TokenLength, TokenText, Success);

        public required TokenDefinition TokenType { get; init; }
        public required int TokenPosition { get; init; }
        public required int TokenLength { get; init; }
        public required string TokenText { get; init; }
        public required bool Success { get; init; }

        public bool Equals(TokenMatch other) =>
            TokenType.Equals(other.TokenType) && TokenPosition == other.TokenPosition && TokenLength == other.TokenLength && TokenText == other.TokenText && Success == other.Success;

        public static bool operator ==(TokenMatch left, TTokenEnum? right) =>
            EqualityComparer<TTokenEnum>.Default.Equals(left.TokenType.TokenId, right);

        public static bool operator !=(TokenMatch left, TTokenEnum? right) =>
            !(left == right);
    }

    /// <summary>
    /// Defines a token within the lexer.
    /// Note: There should only be one instance of each token
    /// </summary>
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

    /// <summary>
    /// Delegate for a lexing event when a token is matched.
    /// </summary>
    public delegate void TokenMatched(TokenMatch match, IBisLexer<TTokenEnum> lexer);

    /// <summary>
    /// Event triggered when a token is matched.
    /// </summary>
    public event TokenMatched? OnTokenMatched;

    protected IEnumerable<TokenDefinition> TokenTypes { get; }
    protected IEnumerable<TokenMatch> PreviousMatches { get; }

    protected TokenDefinition? ErrorToken { get; }
    protected TokenDefinition EOFToken { get; }


    public TokenMatch? PreviousMatch();

}

/// <summary>
/// A basic implementation of the IBisLexer interface.
/// </summary>
public abstract class BisLexer<TTokenEnum> : BisMutableStringStepper, IBisLexer<TTokenEnum> where TTokenEnum : Enum
{
    public event IBisLexer<TTokenEnum>.TokenMatched? OnTokenMatched;
    public abstract IEnumerable<IBisLexer<TTokenEnum>.TokenDefinition> TokenTypes { get; }
    public abstract IBisLexer<TTokenEnum>.TokenDefinition? ErrorToken { get; }
    public abstract IBisLexer<TTokenEnum>.TokenDefinition EOFToken { get; }
    public IEnumerable<IBisLexer<TTokenEnum>.TokenMatch> PreviousMatches => previousMatches;
    private readonly List<IBisLexer<TTokenEnum>.TokenMatch> previousMatches = new();

    protected BisLexer(string content) : base(content)
    {
    }

    public IBisLexer<TTokenEnum>.TokenMatch? PreviousMatch() => PreviousMatches.LastOrDefault();

    public Result ProcessLexer<TPreprocessor>(TPreprocessor? preprocessor) where TPreprocessor : BisPreProcessorBase, new()
    {
        preprocessor ??= new TPreprocessor();
        var builder = new StringBuilder();
        var preprocessResult = preprocessor.EvaluateLexer(this, builder);
        ResetLexer(builder.ToString());
        return preprocessResult;
    }

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
        previousMatches.Add(match);
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
