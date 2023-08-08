namespace BisUtils.Core.Parsing.Parser;

using Lexer;
using Processing;
using Token.Typing;

public static class BisParserExtensions
{
    public static void ProcessAndParse<TSyntaxTree, TLexer, TTokens, TProcessor>(
        this IBisParser<TSyntaxTree, TLexer, TTokens> parser, TLexer lexer, TProcessor? processor)
        where TLexer : BisLexer<TTokens>
        where TTokens : BisTokenTypeSet<TTokens>, new()
        where TProcessor : IBisPreProcessor<TTokens>
    {
        processor?.ProcessLexer(lexer);
        lexer.ResetLexer();
        parser.Parse(lexer);
    }

}
