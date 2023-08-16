namespace BisUtils.EnLex.Tokens;

using Core.ParsingFramework.Tokens.Type;
using Core.ParsingFramework.Tokens.Type.Types;
using Core.ParsingFramework.Tokens.TypeSet;

// ReSharper disable file StaticMemberInGenericType
public class EnfusionTokenSet<T> : BisTokenTypeSet<T> where T : EnfusionTokenSet<T>, new()
{
    public static readonly IBisTokenType EnfusionNewLine =
        new BisEOLTokenType("enfusion");

    public static readonly IBisTokenType EnfusionWhitespace =
        new BisTokenType("enfusion.abstract.whitespace", "[\t ]");

    public static readonly IBisTokenType EnfusionDelimitedComment =
        new BisTokenType("enfusion.comment.delimited.left", @"/\*.*\*/");

    public static readonly IBisTokenType EnfusionLineComment =
        new BisTokenType("enfusion.comment.line", "//");

    public static readonly IBisTokenType EnfusionLineMacro =
        new BisTokenType("enfusion.comment.line", "__LINE__");

    public static readonly IBisTokenType EnfusionFileMacro =
        new BisTokenType("enfusion.comment.line", "__FILE__");

    public static readonly IBisTokenType EnfusionHashSymbol =
        new BisTokenType("enfusion.symbol.hash", "#");

    public static readonly IBisTokenType EnfusionLCurly =
        new BisTokenType("enfusion.symbol.curly.left", "{");

    public static readonly IBisTokenType EnfusionRCurly =
        new BisTokenType("enfusion.symbol.curly.right", "}");

    public static readonly IBisTokenType EnfusionColon =
        new BisTokenType("enfusion.symbol.colon", ":");

    public static readonly IBisTokenType EnfusionLiteralString =
        new BisTokenType("enfusion.literal.string", "\".*\"");

    public static readonly IBisTokenType EnfusionIdentifier =
        new BisTokenType("enfusion.identifier", "[A-Za-z_] [0-9A-Za-z_]*");

}

public class EnfusionTokenSet : EnfusionTokenSet<EnfusionTokenSet>
{

    public EnfusionTokenSet()
    {

    }

}
