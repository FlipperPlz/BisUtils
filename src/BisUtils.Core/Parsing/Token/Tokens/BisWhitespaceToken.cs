namespace BisUtils.Core.Parsing.Token.Tokens;

public class BisWhitespaceTokenType : IBisTokenType
{
    public static readonly BisWhitespaceTokenType Instance = new BisWhitespaceTokenType();

    public string TokenType => "Whitespace";
    public string TokenRegex => "[\t\r\n ]";
}
