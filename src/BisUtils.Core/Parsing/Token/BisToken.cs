namespace BisUtils.Core.Parsing.Token;

public interface IBisToken
{
    string TokenType { get; }
    string TokenRegex { get; } // used for debugging
}


public readonly struct BisToken : IBisToken
{
    public string TokenType { get; }
    public string TokenRegex { get; }

    public BisToken(string tokenType, string tokenRegex)
    {
        TokenType = tokenType;
        TokenRegex = tokenRegex;
    }
}
