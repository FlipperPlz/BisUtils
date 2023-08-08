namespace BisUtils.Core.Parsing;

using System.Text;
using BisUtils.Core.Parsing.Lexer;
using FResults;
using Microsoft.Extensions.Logging;

public static class ParserExtensions
{
    public static Result ProcessAndParse<TAstNode, TLexer, TTypes, TPreProcessor>
    (
        this IBisParserOld<TAstNode, TLexer, TTypes, TPreProcessor> parserOld,
        out TAstNode? node,
        TLexer lexer,
        ILogger? logger,
        TPreProcessor? preprocessor = null
    ) where TLexer : BisLexerOld<TTypes> where TTypes : Enum where TPreProcessor : BisPreProcessorBase, new()
    {
        var builder = new StringBuilder();
        preprocessor ??= new TPreProcessor();
        preprocessor.EvaluateLexer(lexer, builder);
        lexer.ResetLexer(builder.ToString());
        return parserOld.Parse(out node, lexer, logger);
    }
}
