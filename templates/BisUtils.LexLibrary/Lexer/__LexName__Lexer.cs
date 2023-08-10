namespace BisUtils.LexLibrary.Lexer;

using Core.Parsing.Lexer;
using Core.Parsing.Token.Typing;
using Tokens;

public interface I__LexName__Lexer<out TTokens> : IBisLexer<TTokens> where TTokens : __LexName__TokenSet<TTokens>
{
    public IBisTokenType TryMatchNewLine();
}

public abstract class __LexName__Lexer<TTokens> : BisLexerAbs<TTokens>, I__LexName__Lexer<TTokens>
    where TTokens : __LexName__TokenSet<TTokens>, new()
{
    protected __LexName__Lexer(string content) : base(content, true)
    {
    }

    protected abstract override IBisTokenType LocateExtendedMatch(int tokenStart, char? currentChar);

    protected override IBisTokenType LocateNextMatch(int tokenStart, char? currentChar) =>
        currentChar switch
        {
            '\r' or '\n' => TryMatchNewLine(),
            _ => InvalidToken
        };

    public IBisTokenType TryMatchNewLine()
    {
        switch (CurrentChar)
        {
            case '\r':
            {
                if (PeekForward() != '\n')
                {
                    return InvalidToken;
                }

                MoveForward();
                return __LexName__TokenSet.__LexName__NewLine;
            }
            case '\n': return __LexName__TokenSet.__LexName__NewLine;
            default: return InvalidToken;
        }
    }
}
