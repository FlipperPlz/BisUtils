namespace BisUtils.Core.ParsingFramework.Tokens.Type;

public interface IBisTokenType
{
    string TokenType { get; }
    string TokenRegex { get; } // used for debugging
}
