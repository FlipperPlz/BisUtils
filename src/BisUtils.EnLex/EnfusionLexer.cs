namespace BisUtils.EnLex;

using Core.Parsing.Lexer;
using Core.Parsing.Token.Matching;
using Core.Parsing.Token.Tokens;
using Core.Parsing.Token.Typing;

public interface IEnfusionLexer<out TTokens> : IBisLexer<TTokens> where TTokens : EnfusionTokenSet<TTokens>
{

    public IBisTokenType TryMatchComment(out string commentText);
}

public class EnfusionLexer<TTokens> : BisLexer<TTokens>, IEnfusionLexer<TTokens> where TTokens : EnfusionTokenSet<TTokens>, new()
{
    public EnfusionLexer(string content) : base(content)
    {
    }

    protected override IBisTokenType LocateNextMatch(int tokenStart) =>
        MoveForward() switch
        {
            '/' => TryMatchComment(out _),
            _ => BisInvalidTokeType.Instance
        };

    public IBisTokenType TryMatchComment(out string commentText)
    {
        if (CurrentChar == '/' && MoveForward() == '/')
        {
            commentText = GetWhile(_ => CurrentChar != '\n');
            return EnfusionTokenSet.EnfusionLineComment;
        }

        if (CurrentChar == '*')
        {
            commentText = MoveForward() == '/'
                ? string.Empty
                : GetWhile(_ => !(PreviousChar == '*' && CurrentChar == '/'));
            return EnfusionTokenSet.EnfusionDelimitedComment;

        }

        commentText = string.Empty;
        return BisInvalidTokeType.Instance;
    }
}
