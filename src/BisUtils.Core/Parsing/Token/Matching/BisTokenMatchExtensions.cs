namespace BisUtils.Core.Parsing.Token.Matching;

public static class BisTokenMatchExtensions
{

    public static Range GetTokenLocation(this IBisTokenMatch match) =>
        match.TokenPosition..(match.TokenPosition + match.TokenLength);
}
