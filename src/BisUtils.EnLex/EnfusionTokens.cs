namespace BisUtils.EnLex;

using Core.Parsing.Token;

// ReSharper disable file StaticMemberInGenericType
public class EnfusionTokens<T> : BisTokenSet<T> where T : EnfusionTokens<T>
{
    public static readonly IBisToken EnfusionWhitespace =
        new BisToken("enfusion.whitespace", "[\t\n\r]");

    public static readonly IBisToken EnfusionDelimitedCommentStart =
        new BisToken("enfusion.comment.delimited.left", "/*");

    public static readonly IBisToken EnfusionDelimitedCommentEnd =
        new BisToken("enfusion.comment.delimited.right", "*/");

    public static readonly IBisToken EnfusionLineCommentStart =
        new BisToken("enfusion.comment.line", "//");

    public static readonly IBisToken EnfusionLineMacro =
        new BisToken("enfusion.comment.line", "__LINE__");

    public static readonly IBisToken EnfusionFileMacro =
        new BisToken("enfusion.comment.line", "__FILE__");

    public static readonly IBisToken EnfusionHashSymbol =
        new BisToken("enfusion.symbol.hash", "#");

    public static readonly IBisToken EnfusionLiteralString =
        new BisToken("enfusion.literal.string", "\".*\"");

    public static readonly IBisToken EnfusionIdentifier =
        new BisToken("enfusion.identifier", "[A-Za-z_] [0-9A-Za-z_]*");

}

public class EnfusionTokens : EnfusionTokens<EnfusionTokens>
{

}
