namespace BisUtils.RvLex.Tokens;

using Core.Parsing.Token.Tokens;
using Core.Parsing.Token.Typing;

// ReSharper disable file StaticMemberInGenericType
public class RvTokenSet<T> : BisTokenTypeSet<T> where T : RvTokenSet<T>
{
    public static readonly IBisTokenType RvNewLine =
        new BisCustomEOLTokenType("rv", "\n");

    public static readonly IBisTokenType RvComma =
        new BisCustomEOLTokenType("rv.symbols.comma", ",");

    public static readonly IBisTokenType RvIdentifier =
        new BisTokenType("rv.identifier", "[A-Za-z_] [0-9A-Za-z_]*");
}

public sealed class RvTokenSet : RvTokenSet<RvTokenSet>
{

    private RvTokenSet()
    {

    }

}

