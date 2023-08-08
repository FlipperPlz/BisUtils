namespace BisUtils.Core.Parsing.Lexer;

using System.Text;
using Extensions;
using Token.Matching;
using Token.Tokens;
using Token.Typing;

public interface IBisLexer : IBisMutableStringStepper
{
    public IEnumerable<IBisTokenMatch> PreviousMatches { get; }
    public event EventHandler<IBisTokenMatch> OnTokenMatched;

    public IBisTokenMatch LexToken();

}

public interface IBisLexer<out TTokens> : IBisLexer where TTokens : IBisTokenTypeSet
{
    public TTokens LexicalTokenSet { get; }
}


public abstract class BisLexer<TTokens> : BisMutableStringStepper, IBisLexer<TTokens> where TTokens : BisTokenTypeSet<TTokens>, new()
{
    private readonly List<IBisTokenMatch> previousMatches = new();
    public IEnumerable<IBisTokenMatch> PreviousMatches => previousMatches;
    public event EventHandler<IBisTokenMatch> OnTokenMatched = delegate {  };

    public TTokens LexicalTokenSet => BisTokenExtensions.FindTokenSet<TTokens>();

    protected BisLexer(string content) : base(content) => this.TokenizeUntilEnd();

    public IBisTokenMatch LexToken() => RegisterNextMatch(Position);

    private IBisTokenMatch RegisterNextMatch(int tokenStart)
    {
        //we pass our token start because we are so nice and thoughtful towards the people using
        //my shitty frameworks:)
        //TODO: Maybe handle EOF for these lovely individuals xP
        var type = LocateNextMatch(tokenStart);

        var match = type is BisInvalidTokeType or null
            ? CreateInvalidMatch(tokenStart)
            : CreateTokenMatch(type, tokenStart);
        previousMatches.Add(match);
        OnTokenMatched.Invoke(this, match);
        return match;
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

    protected IBisTokenMatch CreateTokenMatch(IBisTokenType type, int tokenStart)
    {
        var text = GetRange(tokenStart..Position);
        return new BisTokenMatch(this, type, text, tokenStart, text.Length);
    }

    protected IBisTokenMatch CreateInvalidMatch(int tokenStart) =>
        CreateTokenMatch(BisInvalidTokeType.Instance, tokenStart);

    protected abstract IBisTokenType? LocateNextMatch(int tokenStart);
}
