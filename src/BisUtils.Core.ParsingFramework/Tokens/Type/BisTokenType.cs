namespace BisUtils.Core.ParsingFramework.Tokens.Type;

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

