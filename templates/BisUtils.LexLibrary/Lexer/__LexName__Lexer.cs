namespace BisUtils.LexLibrary.Lexer;

using Core.Parsing.Lexer;
using Core.Parsing.Token.Tokens;
using Core.Parsing.Token.Typing;
using Tokens;

public interface I__LexName__Lexer<out TTokens> : IBisLexer<TTokens> where TTokens : __LexName__TokenSet<TTokens>
{
    public IBisTokenType TryMatchNewLine();
}

public class __LexName__Lexer<TTokens> : BisLexer<TTokens>, I__LexName__Lexer<TTokens>
    where TTokens : __LexName__TokenSet<TTokens>, new()
{
    public __LexName__Lexer(string content) : base(content)
    {
    }

    protected override IBisTokenType LocateNextMatch(int tokenStart) =>
        MoveForward() switch
        {
            '\r' or '\n' => TryMatchNewLine(),
            _ => BisInvalidTokeType.Instance
        };

    public IBisTokenType TryMatchNewLine()
    {
        switch (CurrentChar)
        {
            case '\r':
            {
                if (PeekForward() != '\n')
                {
                    return BisInvalidTokeType.Instance;
                }

                MoveForward();
                return __LexName__TokenSet.__LexName__NewLine;
            }
            case '\n': return __LexName__TokenSet.__LexName__NewLine;
            default: return BisInvalidTokeType.Instance;
        }
    }
}
