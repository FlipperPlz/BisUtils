namespace BisLexicalLibrary.Tokens;

using BisUtils.Core.Parsing.Token.Typing;

public class LanguageNameTokens<T> : BisTokenTypeSet<T> where T : LanguageNameTokens<T>
{
    public static readonly IBisTokenType LanguageNameDickButt =
        new BisTokenType("LanguageName.dickbutt", ".*");

}

public sealed class LanguageNameTokens : LanguageNameTokens<LanguageNameTokens>
{

    private LanguageNameTokens()
    {

    }

}
