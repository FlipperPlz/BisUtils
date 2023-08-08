namespace BisUtils.Core.Parsing.Token;

public interface IBisTokenType
{
    string TokenType { get; }
    string TokenRegex { get; } // used for debugging
}


public readonly struct BisTokenType : IBisTokenType
{
    public string TokenType { get; }
    public string TokenRegex { get; }

    public BisTokenType(string tokenType, string tokenRegex)
    {
        TokenType = tokenType;
        TokenRegex = tokenRegex;
    }
}
