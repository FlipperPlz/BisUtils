namespace BisUtils.Core.Parsing.Token.Tokens;

using Typing;

public class BisInvalidTokeType : IBisTokenType
{
    public static readonly BisInvalidTokeType Instance = new BisInvalidTokeType();
    public string TokenType => "__ERR__";
    public string TokenRegex => ".*";
}
