namespace BisUtils.Core.ParsingFramework.Tokens.Type;

public abstract class BisTokenTypeCore : IBisTokenType
{
    public abstract string TokenType { get; }
    public abstract string TokenRegex { get; }
}
