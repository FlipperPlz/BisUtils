namespace BisUtils.EnConfig.Lexer;

using Core.ParsingFramework.Tokens.Type;
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
