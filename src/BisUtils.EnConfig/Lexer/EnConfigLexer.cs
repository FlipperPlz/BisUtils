namespace BisUtils.EnConfig.Lexer;

using EnLex.Lexer;
using LangAssembler.Models.Doc;
using Tokens;


public sealed class EnConfigLexer : EnfusionLexer<EnConfigTokenSet>
{
    public EnConfigLexer(Document document, bool leaveOpen = false) : base(document, leaveOpen)
    {
    }
}
