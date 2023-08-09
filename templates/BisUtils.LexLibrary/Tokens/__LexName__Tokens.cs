namespace BisUtils.LexLibrary.Tokens;

using Core.Parsing.Token.Tokens;
using Core.Parsing.Token.Typing;

// ReSharper disable file StaticMemberInGenericType
public class __LexName__TokenSet<T> : BisTokenTypeSet<T> where T : __LexName__TokenSet<T>
{
    public static readonly IBisTokenType __LexName__NewLine =
        new BisEOLTokenType("__LexName__");

}

public sealed class __LexName__TokenSet : __LexName__TokenSet<__LexName__TokenSet>
{

    private __LexName__TokenSet()
    {

    }

}

