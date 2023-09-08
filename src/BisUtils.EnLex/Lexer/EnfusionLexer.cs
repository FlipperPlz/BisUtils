namespace BisUtils.EnLex.Lexer;

using LangAssembler.Lexer;
using LangAssembler.Models.Doc;
using Tokens;


public class EnfusionLexer<TTokens> : TokenSetLexer<TTokens> where TTokens : EnfusionTokenSet
{
    public EnfusionLexer(Document document, bool leaveOpen = false) : base(document, leaveOpen)
    {
    }
}
