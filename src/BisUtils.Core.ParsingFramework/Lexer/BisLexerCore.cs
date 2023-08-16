namespace BisUtils.Core.ParsingFramework.Lexer;

using System.Text;
using Extensions;
using Microsoft.Extensions.Logging;
using Misc;
using Steppers.Mutable;
using Tokens.Match;
using Tokens.Type;
using Tokens.Type.Types;

public abstract class BisLexerCore : BisMutableStringStepper, IBisLexer
{
    protected static readonly IBisTokenType InvalidToken = BisInvalidTokenType.Instance;
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

    protected BisLexerCore(BinaryReader content, Encoding encoding, StepperDisposalOption option, ILogger? logger = default, int? length = null, long? stringStart = null) :
        base(content, encoding, option, logger, length, stringStart)
    {
    }

    public BisTokenMatch LexToken() => RegisterNextMatch(Position);

    protected BisTokenMatch RegisterNextMatch(int tokenStart)
    {
        //we pass our token start because we are so nice and thoughtful towards the people using
        //my shitty frameworks :)

        var type = FindNextMatch(tokenStart, MoveForward());
        var match = type is BisInvalidTokenType or null
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
        if (position < GetFarthestIndexed())
        {
            throw new NotSupportedException();
        }

        return base.JumpTo(position);
    }

    private int GetFarthestIndexed()
    {
        if (!PreviousMatches.Any())
        {
            return -1;
        }

        var last = PreviousMatches.LastOrDefault();

        return last.TokenPosition + last.TokenLength;
    }

    protected IBisTokenType TryMatchWord(string word, IBisTokenType validType)
    {
        var wordLength = word.Length;
        if (this.PeekForwardMulti(wordLength) != word)
        {
            return InvalidToken;
        }

        MoveForward(wordLength - 1);
        return validType;

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
            return InvalidToken;
        }

        while (peekedDelegate(this.PeekForward()))
        {
            MoveForward();
        }

        return validTokenType;
    }

    public override void ResetStepper(string? content = null)
    {
        base.ResetStepper(content);
        previousMatches.Clear();
    }

    protected IBisTokenType TryMatchText(string expectedText, IBisTokenType tokenType, bool consumeCurrent = false)
    {

        var start = consumeCurrent ? 1 : 0;
        var matchText = consumeCurrent ? CurrentChar + this.PeekForwardMulti(expectedText.Length - start) : this.PeekForwardMulti(expectedText.Length);

        if (matchText != expectedText)
        {
            return InvalidToken;
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
        CreateTokenMatch(InvalidToken, tokenStart);

    protected IBisTokenType TryMatchChar(char c, IBisTokenType validTokenType) => CurrentChar == c ? validTokenType : InvalidToken;
}
