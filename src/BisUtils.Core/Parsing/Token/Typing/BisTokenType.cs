namespace BisUtils.Core.Parsing.Token.Typing;

public interface IBisTokenType
{
    string TokenType { get; }
    string TokenRegex { get; } // used for debugging
}


public abstract class BisTokenTypeCore : IBisTokenType
{
    public abstract string TokenType { get; }
    public abstract string TokenRegex { get; }
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
