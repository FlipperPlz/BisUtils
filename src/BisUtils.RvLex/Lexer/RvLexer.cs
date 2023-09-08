namespace BisUtils.RvLex.Lexer;

using LangAssembler.Lexer;
using LangAssembler.Models.Doc;
using Tokens;

public class RvLexer<TTokens> : TokenSetLexer<TTokens> where TTokens : RvTokenSet
{
    public RvLexer(Document document, bool leaveOpen = false) : base(document, leaveOpen)
    {
    }
}
