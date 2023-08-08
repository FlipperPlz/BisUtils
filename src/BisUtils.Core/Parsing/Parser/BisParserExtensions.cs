namespace BisUtils.Core.Parsing.Parser;

using Lexer;
using Processing;
using Token.Typing;

public static class BisParserExtensions
{
    public static void ProcessAndParse<TSyntaxTree, TLexer, TTokens, TContext, TProcessor>
    (
        this IBisParser<TSyntaxTree, TLexer, TTokens, TContext> parser,
        TLexer lexer,
        TProcessor? processor
    )
        where TSyntaxTree : new()
        where TLexer : BisLexer<TTokens>
        where TTokens : BisTokenTypeSet<TTokens>, new()
        where TContext : IBisParserContext, new()
        where TProcessor : IBisPreProcessor<TTokens>
    {
        processor?.ProcessLexer(lexer);
        lexer.ResetLexer();
        parser.Parse(lexer);
    }

    public static bool ShouldContinue(this IBisParserContext context) => !context.ShouldEnd;

}
