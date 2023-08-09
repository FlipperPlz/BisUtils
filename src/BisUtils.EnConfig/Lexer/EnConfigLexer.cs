namespace BisUtils.EnConfig.Lexer;

using EnLex;
using EnLex.Lexer;
using Tokens;

public interface IEnConfigLexer : IEnfusionLexer<EnConfigTokenSet>
{

}

public sealed class EnConfigLexer : EnfusionLexer<EnConfigTokenSet>, IEnConfigLexer
{
    public EnConfigLexer(string content) : base(content)
    {
    }

}
