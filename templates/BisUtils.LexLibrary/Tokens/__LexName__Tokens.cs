namespace BisUtils.LexLibrary.Tokens;

using Core.ParsingFramework.Tokens.Type;
using Core.ParsingFramework.Tokens.Type.Types;
using Core.ParsingFramework.Tokens.TypeSet;

// ReSharper disable file StaticMemberInGenericType
public class __LexName__TokenSet<T> : BisTokenTypeSet<T> where T : __LexName__TokenSet<T>, new()
{
    public static readonly IBisTokenType __LexName__NewLine =
        new BisEOLTokenType("__LexName__");

}

public sealed class __LexName__TokenSet : __LexName__TokenSet<__LexName__TokenSet>
{
    public __LexName__TokenSet()
    {

    }

}

