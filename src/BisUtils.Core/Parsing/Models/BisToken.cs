namespace BisUtils.Core.Parsing.Models;

public readonly ref struct BisToken
{
    public long TokenPosition { get; init; }
    public short TokenId { get; init; }
    public string TokenText { get; init; }
}
