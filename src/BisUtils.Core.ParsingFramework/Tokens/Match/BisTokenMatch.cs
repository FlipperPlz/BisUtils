namespace BisUtils.Core.ParsingFramework.Tokens.Match;

using Extensions;
using Lexer;
using Type;

public struct BisTokenMatch : IBisTokenMatch
{
    private readonly IBisLexer lexer;

    public IBisTokenType TokenType { get; private set; }
    public string TokenText { get; private set; }
    public int TokenPosition { get; }
    public int TokenLength { get; private set; }

    public BisTokenMatch(IBisLexer lexer, IBisTokenType type, string text, int pos, int length)
    {
        this.lexer = lexer;

        TokenType = type;
        TokenText = text;
        TokenPosition = pos;
        TokenLength = length;
    }
    public static explicit operator BisTokenType(BisTokenMatch match) =>
        // please replace this with your own conversion logic
        (BisTokenType)match.TokenType;

    public void RemoveToken() => lexer.RemoveRange(this.GetTokenLocation(), out _);

    public void SetTokenText(string text)
    {
        lexer.ReplaceRange(this.GetTokenLocation(), TokenText = text, out _);
        TokenLength = text.Length;
    }

    public void ReassignToken(IBisTokenType type) => TokenType = type;
}
