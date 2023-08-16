namespace BisUtils.Core.ParsingFramework.Tokens.Match;

using Type;

public interface IBisTokenMatch
{
    public IBisTokenType TokenType { get; }
    public string TokenText { get; }
    public int TokenPosition { get; }
    public int TokenLength { get; }
}
