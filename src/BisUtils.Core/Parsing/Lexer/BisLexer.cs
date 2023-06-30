namespace BisUtils.Core.Parsing.Lexer;

using System.Text;
using FResults;


/// <summary>
/// Interface for defining the basic functionalities of a lexer.
/// </summary>
/// <typeparam name="TTokenEnum">The type of tokens that the lexer will generate.</typeparam>
public interface IBisLexer<TTokenEnum> where TTokenEnum : Enum
{
    /// <summary>
    /// Struct that represents a single Token Match instance.
    /// </summary>
    public readonly struct TokenMatch {

        public override bool Equals(object? obj) => obj is TokenMatch other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(TokenType, TokenPosition, TokenLength, TokenText, Success);

        /// <summary>
        /// Gets or sets the token type.
        /// </summary>
        public required TokenDefinition TokenType { get; init; }

        /// <summary>
        /// Gets or sets the token position.
        /// </summary>
        public required int TokenPosition { get; init; }

        /// <summary>
        /// Gets or sets the length of the token.
        /// </summary>
        public required int TokenLength { get; init; }

        /// <summary>
        /// Gets or sets the text of the token.
        /// </summary>
        public required string TokenText { get; init; }

        /// <summary>
        /// Gets or sets the success state of the token matching.
        /// </summary>
        public required bool Success { get; init; }

        public bool Equals(TokenMatch other) =>
            TokenType.Equals(other.TokenType) && TokenPosition == other.TokenPosition && TokenLength == other.TokenLength && TokenText == other.TokenText && Success == other.Success;

        public static implicit operator TTokenEnum(TokenMatch d) => d.TokenType;

        public static bool operator ==(TokenMatch left, TTokenEnum? right) =>
            EqualityComparer<TTokenEnum>.Default.Equals(left.TokenType.TokenId, right);

        public static bool operator !=(TokenMatch left, TTokenEnum? right) =>
            !(left == right);
    }

    /// <summary>
    /// Struct that represents a single Token Definition instance.
    /// </summary>
    public readonly struct TokenDefinition
    {
        public bool Equals(TokenDefinition other) =>
            DebugName == other.DebugName && EqualityComparer<TTokenEnum>.Default.Equals(TokenId, other.TokenId) && TokenWeight == other.TokenWeight;

        public override bool Equals(object? obj) => obj is TokenDefinition other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(DebugName, TokenId, TokenWeight);

        /// <summary>
        /// Gets or sets the debug name of the token.
        /// </summary>
        public required string? DebugName { get; init; }

        /// <summary>
        /// Gets or sets the token id.
        /// </summary>
        public required TTokenEnum TokenId { get; init; }

        /// <summary>
        /// Gets or sets the token weight.
        /// </summary>
        public required short TokenWeight { get; init; }

        public static implicit operator TTokenEnum(TokenDefinition d) => d.TokenId;

        public static bool operator ==(TokenDefinition left, TTokenEnum? right) =>
            EqualityComparer<TTokenEnum>.Default.Equals(left.TokenId, right);

        public static bool operator !=(TokenDefinition left, TTokenEnum? right) =>
            !(left == right);
    }

    /// <summary>
    /// Delegate for a lexing event fired when a token is matched.
    /// </summary>
    /// <param name="match">The matched token.</param>
    /// <param name="lexer">The lexer instance handling the input.</param>
    public delegate void TokenMatched(TokenMatch match, IBisLexer<TTokenEnum> lexer);

    /// <summary>
    /// Event triggered when a token is matched.
    /// </summary>
    public event TokenMatched? OnTokenMatched;

    /// <summary>
    /// Gets or sets the defined token types within the lexer.
    /// </summary>
    protected IEnumerable<TokenDefinition> TokenTypes { get; }

    /// <summary>
    /// Gets or sets a list of previously matched tokens.
    /// </summary>
    protected IEnumerable<TokenMatch> PreviousMatches { get; }

    /// <summary>
    /// Gets the token that represents an error state.
    /// </summary>
    protected TokenDefinition? ErrorToken { get; }

    /// <summary>
    /// Gets the End of file token definition.
    /// </summary>
    protected TokenDefinition EOFToken { get; }

    /// <summary>
    /// Returns the previously matched token.
    /// </summary>
    /// <returns>The previously matched token.</returns>
    public TokenMatch? PreviousMatch();

}


/// <summary>
/// Basic implementation of the IBisLexer interface.
/// </summary>
/// <typeparam name="TTokenEnum">The type of tokens the lexer will generate.</typeparam>
public abstract class BisLexer<TTokenEnum> : BisMutableStringStepper, IBisLexer<TTokenEnum> where TTokenEnum : Enum
{
    public event IBisLexer<TTokenEnum>.TokenMatched? OnTokenMatched;

    /// <summary>
    /// An abstract member representing the defined token types in lexer.
    /// </summary>
    public abstract IEnumerable<IBisLexer<TTokenEnum>.TokenDefinition> TokenTypes { get; }

    /// <summary>
    /// An abstract member representing Error token type in lexer.
    /// </summary>
    public abstract IBisLexer<TTokenEnum>.TokenDefinition? ErrorToken { get; }

    /// <summary>
    /// An abstract member representing EOF token type in lexer.
    /// </summary>
    public abstract IBisLexer<TTokenEnum>.TokenDefinition EOFToken { get; }

    /// <summary>
    /// A list of previously matched tokens.
    /// </summary>
    public IEnumerable<IBisLexer<TTokenEnum>.TokenMatch> PreviousMatches => previousMatches;
    private readonly List<IBisLexer<TTokenEnum>.TokenMatch> previousMatches = new();

    protected BisLexer(string content) : base(content)
    {
    }

    /// <summary>
    /// Fetches the previously matched token.
    /// </summary>
    /// <returns>The previously matched token.</returns>
    public IBisLexer<TTokenEnum>.TokenMatch? PreviousMatch() => PreviousMatches.LastOrDefault();

    /// <summary>
    /// Processes the lexer with a preprocessor.
    /// </summary>
    /// <typeparam name="TPreprocessor">The type of preprocessor.</typeparam>
    /// <param name="preprocessor">The preprocessor instance or null if a new one should be created.</param>
    /// <returns>The result of the processing.</returns>
    public Result ProcessLexer<TPreprocessor>(TPreprocessor? preprocessor) where TPreprocessor : BisPreProcessorBase, new()
    {
        preprocessor ??= new TPreprocessor();
        var builder = new StringBuilder();
        var preprocessResult = preprocessor.EvaluateLexer(this, builder);
        ResetLexer(builder.ToString());

        return preprocessResult;
    }

    /// <summary>
    /// Retrieves the next token from the sequence.
    /// </summary>
    /// <returns>The next token.</returns>
    public IBisLexer<TTokenEnum>.TokenMatch NextToken()
    {
        var value = GetNextToken();
        OnTokenMatchedHandler(value, this);
        return value;
    }

    /// <summary>
    /// Tokenizes the entire sequence until the end.
    /// </summary>
    public void TokenizeUntilEnd()
    {
        while (NextToken() != EOFToken.TokenId)
        {
        }
    }

    /// <summary>
    /// Tokenizes the sequence while the given condition is met.
    /// </summary>
    /// <param name="type">The token type for the condition.</param>
    /// <param name="reverse">If set to true, will tokenize until the given token is found.</param>
    /// <returns>The last matched token.</returns>
    public IBisLexer<TTokenEnum>.TokenMatch TokenizeWhile(TTokenEnum type, bool reverse = false)
    {
        IBisLexer<TTokenEnum>.TokenMatch token;
        if (reverse)
        {
            while ((token = NextToken()) != type )
            {
            }
        }
        else
        {
            while ((token = NextToken()) == type )
            {
            }
        }

        return token;
    }

    /// <summary>
    /// Handles when a token is matched.
    /// </summary>
    /// <param name="match">The matched token.</param>
    /// <param name="lexer">The lexer instance handling the input.</param>
    protected virtual void OnTokenMatchedHandler(IBisLexer<TTokenEnum>.TokenMatch match, IBisLexer<TTokenEnum> lexer)
    {
        previousMatches.Add(match);
        OnTokenMatched?.Invoke(match, lexer);
    }

    /// <summary>
    /// Creates a token definition..
    /// </summary>
    protected static IBisLexer<TTokenEnum>.TokenDefinition CreateTokenDefinition(string debugName, TTokenEnum tokenType, short tokenWeight) =>
        new() { DebugName = debugName, TokenId = tokenType, TokenWeight = tokenWeight };

    /// <summary>
    /// Creates a token match based on given range and token definition.
    /// </summary>
    protected IBisLexer<TTokenEnum>.TokenMatch CreateTokenMatch(Range tokenRange, IBisLexer<TTokenEnum>.TokenDefinition tokenDef) =>
        new() {
            Success = true,
            TokenLength = tokenRange.End.Value - tokenRange.Start.Value + 1,
            TokenPosition = tokenRange.Start.Value,
            TokenText = GetRange(tokenRange.Start.Value..(tokenRange.End.Value + 1)),
            TokenType = tokenDef
        };

    /// <summary>
    /// Creates a token match based on the given range, string and token definition.
    /// </summary>
    protected static IBisLexer<TTokenEnum>.TokenMatch CreateTokenMatch(Range tokenRange, string str, IBisLexer<TTokenEnum>.TokenDefinition tokenDef) =>
        new() {
            Success = true,
            TokenLength = tokenRange.End.Value - tokenRange.Start.Value + 1,
            TokenPosition = tokenRange.Start.Value,
            TokenText = str,
            TokenType = tokenDef
        };

    /// <summary>
    /// Abstract method to retrieve the next token from the sequence.
    /// </summary>
    /// <returns>The next token.</returns>
    protected abstract IBisLexer<TTokenEnum>.TokenMatch GetNextToken();

}
