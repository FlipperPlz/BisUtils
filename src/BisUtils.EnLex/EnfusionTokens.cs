namespace BisUtils.EnLex;

using Core.Parsing.Token;

// ReSharper disable file StaticMemberInGenericType
public class EnfusionTokensType<T> : BisTokenTypeSet<T> where T : EnfusionTokensType<T>
{
    public static readonly IBisTokenType EnfusionWhitespace =
        new BisTokenType("enfusion.whitespace", "[\t\n\r]");

    public static readonly IBisTokenType EnfusionDelimitedCommentStart =
        new BisTokenType("enfusion.comment.delimited.left", "/*");

    public static readonly IBisTokenType EnfusionDelimitedCommentEnd =
        new BisTokenType("enfusion.comment.delimited.right", "*/");

    public static readonly IBisTokenType EnfusionLineCommentStart =
        new BisTokenType("enfusion.comment.line", "//");

    public static readonly IBisTokenType EnfusionLineMacro =
        new BisTokenType("enfusion.comment.line", "__LINE__");

    public static readonly IBisTokenType EnfusionFileMacro =
        new BisTokenType("enfusion.comment.line", "__FILE__");

    public static readonly IBisTokenType EnfusionHashSymbol =
        new BisTokenType("enfusion.symbol.hash", "#");

    public static readonly IBisTokenType EnfusionLiteralString =
        new BisTokenType("enfusion.literal.string", "\".*\"");

    public static readonly IBisTokenType EnfusionIdentifier =
        new BisTokenType("enfusion.identifier", "[A-Za-z_] [0-9A-Za-z_]*");

}

public class EnfusionTokensType : EnfusionTokensType<EnfusionTokensType>
{

}
