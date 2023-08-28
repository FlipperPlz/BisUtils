namespace BisUtils.EnLex.Tokens;

using System.Collections;
using LangAssembler.Lexer.Models.Type;
using LangAssembler.Lexer.Models.TypeSet;

// ReSharper disable file StaticMemberInGenericType
public class EnfusionTokenSet : ITokenTypeSet
{
    public static readonly ITokenType EnfusionNewLine =
        new TokenType("enfusion.newline");

    public static readonly ITokenType EnfusionWhitespace =
        new TokenType("enfusion.abstract.whitespace");

    public static readonly ITokenType EnfusionDelimitedComment =
        new TokenType("enfusion.comment.delimited.left");

    public static readonly ITokenType EnfusionLineComment =
        new TokenType("enfusion.comment.line");

    public static readonly ITokenType EnfusionLineMacro =
        new TokenType("enfusion.comment.line");

    public static readonly ITokenType EnfusionFileMacro =
        new TokenType("enfusion.comment.line");

    public static readonly ITokenType EnfusionHashSymbol =
        new TokenType("enfusion.symbol.hash");

    public static readonly ITokenType EnfusionLCurly =
        new TokenType("enfusion.symbol.curly.left");

    public static readonly ITokenType EnfusionRCurly =
        new TokenType("enfusion.symbol.curly.right");

    public static readonly ITokenType EnfusionColon =
        new TokenType("enfusion.symbol.colon");

    public static readonly ITokenType EnfusionLiteralString =
        new TokenType("enfusion.literal.string");

    public static readonly ITokenType EnfusionIdentifier =
        new TokenType("enfusion.identifier");

    public IEnumerator<ITokenType> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
