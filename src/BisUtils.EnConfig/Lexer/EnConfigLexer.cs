namespace BisUtils.EnConfig.Lexer;

using EnLex;
using Tokens;

public interface IEnConfigLexer : IEnfusionLexer<EnConfigTokens>
{

}

public sealed class EnConfigLexer : EnfusionLexer<EnConfigTokens>, IEnConfigLexer
{
    public EnConfigLexer(string content) : base(content)
    {
    }

}
