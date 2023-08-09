namespace BisUtils.EnConfig.Lexer;

using EnLex;
using EnLex.Lexer;
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
