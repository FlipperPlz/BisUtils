namespace BisUtils.Core.Parsing.Lexer;

using Token.Matching;
using Token.Tokens;
using Token.Typing;

public abstract class BisLexerCore : BisMutableStringStepper, IBisLexer
{
    protected static readonly IBisTokenType InvalidToken = BisInvalidTokeType.Instance;
    public event EventHandler<BisTokenMatch> OnTokenMatched = delegate
    {

    };

    public int LineNumber { get; protected set; } = 1;
    public int LineStart { get; protected set; }
    public int ColumnNumber => Position - LineStart;
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool EventsMuted { get; protected set; }
    public BisTokenMatch? LastMatchedToken { get; private set; }

    private readonly List<BisTokenMatch> previousMatches = new();

    public IEnumerable<BisTokenMatch> PreviousMatches => previousMatches;

    protected BisLexerCore(string content) : base(content)
    {
    }

    public BisTokenMatch LexToken() => RegisterNextMatch(Position);

    protected BisTokenMatch RegisterNextMatch(int tokenStart)
    {
        //we pass our token start because we are so nice and thoughtful towards the people using
        //my shitty frameworks :)

        var type = FindNextMatch(tokenStart, MoveForward());
        var match = type is BisInvalidTokeType or null
            ? CreateInvalidMatch(tokenStart)
            : CreateTokenMatch(type, tokenStart);
        TokenMatched(match);
        return match;
    }

    protected virtual IBisTokenType FindNextMatch(int tokenStart, char? currentChar) => LocateNextMatch(tokenStart, currentChar);

    protected void TokenMatched(BisTokenMatch match)
    {
        LastMatchedToken = match;
        AddPreviousMatch(match);
        if (match.TokenType is not BisEOLTokenType)
        {
            return;
        }

        LineStart = match.TokenPosition + match.TokenLength;
        LineNumber++;
        if (!EventsMuted)
        {
            OnTokenMatched.Invoke(this, match);
        }
    }

    protected abstract IBisTokenType LocateNextMatch(int tokenStart, char? currentChar);


    public override char? JumpTo(int position)
    {
        if (position < this.GetFarthestIndexed())
        {
            throw new NotSupportedException();
        }

        return base.JumpTo(position);
    }

    private void AddPreviousMatch(BisTokenMatch match)
    {
        if(previousMatches.Count == 0 || match.TokenPosition >= previousMatches[^1].TokenPosition)
        {
            previousMatches.Add(match);
        }

        var index = previousMatches.BinarySearch(match, Comparer<BisTokenMatch>.Create((x, y) => x.TokenPosition.CompareTo(y.TokenPosition)));
        previousMatches.Insert(index < 0 ? ~index : index, match);
    }

    /// <summary>
    /// Uses binary search to find a token match for a given index
    /// assuming 'previousMatches' is correctly sorted by their positions.
    /// The function operates with a time complexity of O(log n).
    /// </summary>
    /// <param name="tokenIndex">The index of the token to be matched.</param>
    /// <returns>A token match if one exists; null otherwise.</returns>
    /// <remarks>
    /// Given the time complexity of O(log n), use this function
    /// sparingly, especially on larger data sets to avoid performance
    /// implications.
    /// </remarks>
    public BisTokenMatch? MatchForIndex(int tokenIndex)
    {
        int lowerBound = 0, upperBound = previousMatches.Count - 1;
        while (lowerBound <= upperBound)
        {
            var middle = lowerBound + ((upperBound - lowerBound) / 2);
            var currentMatch = previousMatches[middle];

            if
            (
                tokenIndex >= currentMatch.TokenPosition &&
                tokenIndex < currentMatch.TokenPosition + currentMatch.TokenLength
            )
            {
                return currentMatch;
            }

            if (tokenIndex < currentMatch.TokenPosition)
            {
                upperBound = middle - 1;
            }

            lowerBound = middle + 1;
        }

        return null;
    }

    protected IBisTokenType MatchWhile(Func<char?, bool> peekedDelegate, IBisTokenType validTokenType, bool matchCurrent)
    {
        if (matchCurrent && !peekedDelegate(CurrentChar))
        {
            return BisInvalidTokeType.Instance;
        }

        while (peekedDelegate(PeekForward()))
        {
            MoveForward();
        }

        return validTokenType;
    }

    public override void ResetLexer(string? content = null)
    {
        base.ResetLexer(content);
        previousMatches.Clear();
    }

    protected IBisTokenType TryMatchText(string expectedText, IBisTokenType tokenType, bool consumeCurrent = false)
    {

        var start = consumeCurrent ? 1 : 0;
        var matchText = consumeCurrent ? CurrentChar + PeekForwardMulti(expectedText.Length - start) : PeekForwardMulti(expectedText.Length);

        if (matchText != expectedText)
        {
            return BisInvalidTokeType.Instance;
        }

        MoveForward(expectedText.Length - start);
        return tokenType;
    }

    protected BisTokenMatch CreateTokenMatch(IBisTokenType type, int tokenStart)
    {
        var text = GetRange(tokenStart..Position);
        return new BisTokenMatch(this, type, text, tokenStart, text.Length);
    }

    protected BisTokenMatch CreateInvalidMatch(int tokenStart) =>
        CreateTokenMatch(BisInvalidTokeType.Instance, tokenStart);

    protected IBisTokenType TryMatchChar(char c, IBisTokenType validTokenType) => CurrentChar == c ? validTokenType : BisInvalidTokeType.Instance;
}
