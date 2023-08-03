namespace BisUtils.Core.Parsing.Lexer.Tokens;

public interface IBisToken
{
    string TokenType { get; }
}


public readonly struct BisToken : IBisToken
{
    public string TokenType { get; }

    public BisToken(string tokenType) => TokenType = tokenType;
}
