namespace BisUtils.Core.Parsing.Lexer;

using Extensions;
using Token;
using Token.Matching;


public interface IBisLexer : IBisMutableStringStepper
{

    public event EventHandler<IBisTokenMatch> OnTokenMatched;

    public IBisTokenMatch LexToken();
}

public interface IBisLexer<out TTokens> : IBisLexer where TTokens : IBisTokenTypeSet
{
    public TTokens LexicalTokenSet { get; }
}


public abstract class BisLexer<TTokens> : BisMutableStringStepper, IBisLexer<TTokens> where TTokens : BisTokenTypeSet<TTokens>, new()
{
    public event EventHandler<IBisTokenMatch> OnTokenMatched = delegate {  };

    public TTokens LexicalTokenSet => BisTokenExtensions.FindTokenSet<TTokens>();

    protected BisLexer(string content) : base(content) => this.TokenizeUntilEnd();

    public IBisTokenMatch LexToken()
    {
        var token = LocateNextMatch();
        OnTokenMatched.Invoke(this, token);
        return token;
    }


    protected abstract IBisTokenMatch LocateNextMatch();
}
