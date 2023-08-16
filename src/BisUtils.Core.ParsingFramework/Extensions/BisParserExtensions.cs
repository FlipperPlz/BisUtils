namespace BisUtils.Core.ParsingFramework.Extensions;

using Lexer;
using Microsoft.Extensions.Logging;
using Parser;
using Processor;
using Tokens.TypeSet;

public static class BisParserExtensions
{
    public static void ProcessAndParse<TSyntaxTree, TLexer, TTokens, TContext, TProcessor>
    (
        this IBisParser<TSyntaxTree, TLexer, TTokens, TContext> parser,
        TLexer lexer,
        TProcessor? processor,
        ILogger logger
    )
        where TSyntaxTree : new()
        where TLexer : BisLexer<TTokens>
        where TTokens : BisTokenTypeSet<TTokens>, new()
        where TContext : IBisParserContext, new()
        where TProcessor : IBisPreProcessor<TTokens>
    {
        processor?.ProcessLexer(lexer, logger);
        lexer.ResetStepper();
        parser.Parse(lexer, logger);
    }

    public static bool ShouldContinue(this IBisParserContext context) => !context.ShouldEnd;

}
