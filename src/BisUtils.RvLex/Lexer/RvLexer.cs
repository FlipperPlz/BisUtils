namespace BisUtils.RvLex.Lexer;

using Core.Parsing.Lexer;
using Core.Parsing.Token.Tokens;
using Core.Parsing.Token.Typing;
using Tokens;

public interface IRvLexer<out TTokens> : IBisLexer<TTokens> where TTokens : RvTokenSet<TTokens>
{
    public IBisTokenType TryMatchNewLine();
    public IBisTokenType TryMatchComma();
}

public class RvLexer<TTokens> : BisLexer<TTokens>, IRvLexer<TTokens>
    where TTokens : RvTokenSet<TTokens>, new()
{
    public RvLexer(string content) : base(content)
    {
    }

    protected override IBisTokenType LocateNextMatch(int tokenStart) =>
        MoveForward() switch
        {
            '\n' => TryMatchNewLine(),
            ',' => TryMatchComma(),
            _ => BisInvalidTokeType.Instance
        };

    public IBisTokenType TryMatchNewLine() => TryMatchChar('\n', RvTokenSet.RvNewLine);

    public IBisTokenType TryMatchComma() => TryMatchChar(',', RvTokenSet.RvNewLine);
}
