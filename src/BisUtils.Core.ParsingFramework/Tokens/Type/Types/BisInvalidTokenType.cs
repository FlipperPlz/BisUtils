namespace BisUtils.Core.ParsingFramework.Tokens.Type.Types;

public abstract class BisInvalidTokenTypeCore : BisTokenTypeCore
{
    public abstract string TokenSetName { get; }
    public override string TokenRegex => "(\r?\n)";
    public override string TokenType => $"{TokenSetName}.__wrong__";
}

public class BisCustomInvalidTokenType : BisInvalidTokenTypeCore
{
    public override string TokenSetName { get; }
    public override string TokenRegex { get; }


    public BisCustomInvalidTokenType(string tokenSetName, string tokenRegex)
    {
        TokenSetName = tokenSetName;
        TokenRegex = tokenRegex;
    }
}

public class BisInvalidTokenType : BisInvalidTokenTypeCore
{
    public static readonly BisInvalidTokenType Instance = new BisInvalidTokenType("__bis__");
    public override string TokenSetName { get; }

    public BisInvalidTokenType(string tokenSetName) => TokenSetName = tokenSetName;

}

