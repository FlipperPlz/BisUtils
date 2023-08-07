namespace BisUtils.Core.Parsing.Token.Matching;

public interface IBisTokenMatch
{
    public IBisToken TokenMatched { get; }
    public string TokenText { get; }
    public ulong TokenPosition { get; }
    public ulong TokenLength { get; }
    public byte TokenStage { get; }
}

public readonly struct BisTokenMatch : IBisTokenMatch
{
    public IBisToken TokenMatched { get; }
    public string TokenText { get; }
    public ulong TokenPosition { get; }
    public ulong TokenLength { get; }
    public byte TokenStage { get; }

    public BisTokenMatch(IBisToken tokenMatched, string tokenText, ulong tokenPosition, ulong tokenLength, byte tokenStage)
    {
        TokenMatched = tokenMatched;
        TokenText = tokenText;
        TokenPosition = tokenPosition;
        TokenLength = tokenLength;
        TokenStage = tokenStage;
    }
}
