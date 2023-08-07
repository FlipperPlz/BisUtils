namespace BisUtils.Core.Parsing.Lexer;

using Tokens;

public interface IBisLexer<TTokens> : IBisMutableStringStepper where TTokens : IBisTokenSet
{
    public event EventHandler<IBisTokenMatch> OnTokenMatched;

    public IBisTokenMatch LexToken();
}

public abstract class BisLexer<TTokens> : BisMutableStringStepper, IBisLexer<TTokens> where TTokens : IBisTokenSet
{
    public event EventHandler<IBisTokenMatch> OnTokenMatched = delegate {  };

    protected BisLexer(string content) : base(content)
    {
    }


    public IBisTokenMatch LexToken()
    {
        var token = LocateNextMatch();
        OnTokenMatched.Invoke(this, token);
        return token;
    }

    protected abstract IBisTokenMatch LocateNextMatch();
}
