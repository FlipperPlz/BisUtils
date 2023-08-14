namespace BisUtils.RvConfig.Tokens;

using Core.Parsing.Token.Tokens;
using Core.Parsing.Token.Typing;
using Core.Singleton;
using RvLex.Tokens;

public sealed class RvConfigTokenSet : RvTokenSet<RvConfigTokenSet>
{
    public static RvConfigTokenSet Instance => BisSingletonProvider.LocateInstance<RvConfigTokenSet>();

    public static readonly IBisTokenType ConfigNewLine =
        new BisEOLTokenType("param");

    public static readonly IBisTokenType ConfigWhitespace =
        new BisTokenType("param.abstract.whitespace", "[\t ]");

    public static readonly IBisTokenType ConfigLiteral =
        new BisTokenType("param.abstract.literal", ToComplex);

    public static readonly IBisTokenType ConfigLCurly =
        new BisTokenType("param.symbol.curly.left", "{");

    public static readonly IBisTokenType ConfigRCurly =
        new BisTokenType("param.symbol.curly.right", "}");

    public static readonly IBisTokenType ConfigLSquare =
        new BisTokenType("param.symbol.curly.left", "[");

    public static readonly IBisTokenType ConfigRSquare =
        new BisTokenType("param.symbol.curly.right", "]");

    public static readonly IBisTokenType ConfigColon =
        new BisTokenType("param.symbol.colon", ":");

    public static readonly IBisTokenType ConfigSeparator =
        new BisTokenType("param.symbol.separator", ";");

    public static readonly IBisTokenType ConfigAssign =
        new BisTokenType("param.symbol.assign", "=");

    public static readonly IBisTokenType ConfigSubAssign =
        new BisTokenType("param.symbol.assign.subtract", "-=");

    public static readonly IBisTokenType ConfigAddAssign =
        new BisTokenType("param.symbol.assign.add", "+=");

    public static readonly IBisTokenType ConfigClass =
        new BisTokenType("param.keyword.class", "class");

    public static readonly IBisTokenType ConfigDelete =
        new BisTokenType("param.keyword.delete", "delete");

    public static readonly IBisTokenType ConfigEnum =
        new BisTokenType("param.keyword.enum", "enum");
}

