namespace BisUtils.EnLex;

using Core.Parsing.Lexer;
using Core.Parsing.Token.Tokens;
using Core.Parsing.Token.Typing;

public interface IEnfusionLexer<out TTokens> : IBisLexer<TTokens> where TTokens : EnfusionTokenSet<TTokens>
{

    public IBisTokenType TryMatchComment(out string commentText);
    public IBisTokenType TryMatchHash();
    public IBisTokenType TryMatchDirective();
    public IBisTokenType TryMatchCurly(bool isStartCurly);
    public IBisTokenType TryMatchColon();
    public IBisTokenType TryMatchNewLine();

}

public class EnfusionLexer<TTokens> : BisLexer<TTokens>, IEnfusionLexer<TTokens> where TTokens : EnfusionTokenSet<TTokens>, new()
{
    public EnfusionLexer(string content) : base(content)
    {
    }

    protected override IBisTokenType LocateNextMatch(int tokenStart) =>
    MoveForward() switch
    {
        '\r' or '\n'=> TryMatchNewLine(),
        '/' => TryMatchComment(out _),
        '#' => TryMatchHash(),
        '{' => TryMatchCurly(false),
        '}' => TryMatchCurly(true),
        ':' => TryMatchColon(),
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
                return EnfusionTokenSet.EnfusionNewLine;
            }
            case '\n': return EnfusionTokenSet.EnfusionNewLine;
            default: return BisInvalidTokeType.Instance;
        }
    }


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

    public IBisTokenType TryMatchHash()
    {
        if (CurrentChar != '#')
        {
            return BisInvalidTokeType.Instance;
        }

        var directiveType = TryMatchDirective();
        return directiveType is BisInvalidTokeType ? directiveType : EnfusionTokenSet.EnfusionHashSymbol;
    }

    public IBisTokenType TryMatchDirective()
    {
        if (CurrentChar != '#')
        {
            return BisInvalidTokeType.Instance;
        }

        return PeekForward() switch
        {
            'd' => TryMatchText("#define", EnfusionTokenSet.EnfusionDefineDirective),
            'e' => PeekForward(2) switch
            {
                'n' => TryMatchText("#endif", EnfusionTokenSet.EnfusionEndIfDirective),
                'l' => TryMatchText("#else", EnfusionTokenSet.EnfusionElseDirective),
                _ => BisInvalidTokeType.Instance
            },
            'i' => PeekForward(3) switch
            {
                'c' => TryMatchText("#include", EnfusionTokenSet.EnfusionIncludeDirective),
                'd' => TryMatchText("#ifdef", EnfusionTokenSet.EnfusionIfDefinedDirective),
                'n' => TryMatchText("#ifndef", EnfusionTokenSet.EnfusionIfNotDefinedDirective),
                _ => BisInvalidTokeType.Instance
            },
            _ => BisInvalidTokeType.Instance
        };
    }

    public IBisTokenType TryMatchCurly(bool isStartCurly)
    {
        if (isStartCurly)
        {
            return CurrentChar == '{' ? EnfusionTokenSet.EnfusionLCurly : BisInvalidTokeType.Instance;
        }
        return CurrentChar == '}' ? EnfusionTokenSet.EnfusionRCurly : BisInvalidTokeType.Instance;
    }

    public IBisTokenType TryMatchColon() => CurrentChar == ':' ? EnfusionTokenSet.EnfusionIncludeDirective : BisInvalidTokeType.Instance;
}
