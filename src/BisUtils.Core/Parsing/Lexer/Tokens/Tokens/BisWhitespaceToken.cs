namespace BisUtils.Core.Parsing.Lexer.Tokens.Tokens;


public class BisWhitespaceToken : IBisToken
{
    public static readonly BisWhitespaceToken Instance = new BisWhitespaceToken();

    public string TokenType => "Whitespace";
}
