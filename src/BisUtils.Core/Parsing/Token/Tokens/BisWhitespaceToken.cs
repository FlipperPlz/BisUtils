namespace BisUtils.Core.Parsing.Token.Tokens;

public class BisWhitespaceToken : IBisToken
{
    public static readonly BisWhitespaceToken Instance = new BisWhitespaceToken();

    public string TokenType => "Whitespace";
    public string TokenRegex => "[\t\r\n ]";
}
