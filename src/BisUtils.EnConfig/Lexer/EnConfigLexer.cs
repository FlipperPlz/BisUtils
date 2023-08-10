namespace BisUtils.EnConfig.Lexer;

using Core.Parsing.Token.Typing;
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

    protected override IBisTokenType LocateExtendedMatch(int tokenStart, char? currentChar) => throw new NotImplementedException();
}
