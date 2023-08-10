namespace BisUtils.EnLex.Lexer;

using BisUtils.Core.Parsing.Lexer;
using BisUtils.Core.Parsing.Token.Tokens;
using Core.Parsing.Token.Typing;
using Tokens;

public interface IEnfusionLexer<out TTokens> : IBisLexerAbs<TTokens> where TTokens : EnfusionTokenSet<TTokens>, new()
{

    public IBisTokenType TryMatchComment(out string commentText);
    public IBisTokenType TryMatchString(out string stringContent);
    public IBisTokenType TryMatchIdentifier(out string id);
    public IBisTokenType TryMatchHash();
    //public IBisTokenType TryMatchDirective();
    public IBisTokenType TryMatchCurly(bool isStartCurly);
    public IBisTokenType TryMatchColon();
    public IBisTokenType TryMatchNewLine();
    public IBisTokenType TryMatchWhitespace();
}

public abstract class EnfusionLexer<TTokens> : BisLexerAbs<TTokens>, IEnfusionLexer<TTokens> where TTokens : EnfusionTokenSet<TTokens>, new()
{
    protected EnfusionLexer(string content) : base(content, false)
    {
    }


    protected abstract override IBisTokenType LocateExtendedMatch(int tokenStart, char? currentChar);

    protected override IBisTokenType LocateNextMatch(int tokenStart, char? currentChar) => currentChar switch
    {
        '\r' or '\n'=> TryMatchNewLine(),
        '"' => TryMatchString(out _),
        '/' => TryMatchComment(out _),
        '#' => TryMatchHash(),
        '{' => TryMatchCurly(false),
        '}' => TryMatchCurly(true),
        ':' => TryMatchColon(),
        _ => TryMatchIdentifier(out _),
    };

    public IBisTokenType TryMatchString(out string stringContent)
    {
        if (CurrentChar != '"')
        {
            stringContent = string.Empty;
            return BisInvalidTokeType.Instance;
        }

        stringContent = GetWhile(_ => CurrentChar != '"' || PreviousChar == '\\');
        return EnfusionTokenSet.EnfusionLiteralString;
    }

    public IBisTokenType TryMatchIdentifier(out string id)
    {

        if (!IsIdentifierChar(CurrentChar))
        {
            id = string.Empty;
            return TryMatchWhitespace();
        }

        id = ScanUntil(e => !IsIdentifierChar(e), true);
        return EnfusionTokenSet.EnfusionIdentifier;
    }

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

    public IBisTokenType TryMatchWhitespace()
    {
        if (CurrentChar is not (' ' or '\t'))
        {
            return BisInvalidTokeType.Instance;
        }

        ScanUntil(c => c is not (' ' or '\t'));
        return EnfusionTokenSet.EnfusionWhitespace;
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

        // var directiveType = TryMatchDirective();
        return /*directiveType is BisInvalidTokeType ? directiveType :*/ EnfusionTokenSet.EnfusionHashSymbol;
    }

    // public IBisTokenType TryMatchDirective()
    // {
    //     if (CurrentChar != '#')
    //     {
    //         return BisInvalidTokeType.Instance;
    //     }
    //
    //     return PeekForward() switch
    //     {
    //         'd' => TryMatchText("#define", EnfusionTokenSet.EnfusionDefineDirective),
    //         'e' => PeekForward(2) switch
    //         {
    //             'n' => TryMatchText("#endif", EnfusionTokenSet.EnfusionEndIfDirective),
    //             'l' => TryMatchText("#else", EnfusionTokenSet.EnfusionElseDirective),
    //             _ => BisInvalidTokeType.Instance
    //         },
    //         'i' => PeekForward(3) switch
    //         {
    //             'c' => TryMatchText("#include", EnfusionTokenSet.EnfusionIncludeDirective),
    //             'd' => TryMatchText("#ifdef", EnfusionTokenSet.EnfusionIfDefinedDirective),
    //             'n' => TryMatchText("#ifndef", EnfusionTokenSet.EnfusionIfNotDefinedDirective),
    //             _ => BisInvalidTokeType.Instance
    //         },
    //         _ => BisInvalidTokeType.Instance
    //     };
    // }

    public IBisTokenType TryMatchCurly(bool isStartCurly)
    {
        if (isStartCurly)
        {
            return CurrentChar == '{' ? EnfusionTokenSet.EnfusionLCurly : BisInvalidTokeType.Instance;
        }
        return CurrentChar == '}' ? EnfusionTokenSet.EnfusionRCurly : BisInvalidTokeType.Instance;
    }

    public bool IsIdentifierChar(char? idChar, bool isFirst = false)
    {
        if (idChar is not { } currentChar)
        {
            return false;
        }

        if (isFirst && char.IsAsciiDigit(currentChar))
        {
            return false;
        }

        return char.IsAsciiLetter(currentChar) || char.IsAsciiDigit(currentChar) || currentChar is '_';
    }

    public IBisTokenType TryMatchColon() => CurrentChar == ':' ? EnfusionTokenSet.EnfusionColon : BisInvalidTokeType.Instance;
}
