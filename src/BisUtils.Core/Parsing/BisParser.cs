namespace BisUtils.Core.Parsing;

using System.Text;
using FResults;
using Lexer;

#pragma warning disable CA1000
public interface IBisParser<TAstNode, in TLexer, TTypes> where TLexer : BisLexer<TTypes> where TTypes : Enum
{
    public static abstract Result Parse(out TAstNode? node, TLexer lexer);

}

public interface IBisParser<TAstNode, in TLexer, TTypes, in TPreprocessor> : IBisParser<TAstNode, TLexer, TTypes> where TLexer : BisLexer<TTypes> where TPreprocessor : BisPreProcessorBase, new() where TTypes : Enum
{
    // static Result IBisParser<TAstNode, TLexer, TTypes>.Parse(out TAstNode? node, TLexer lexer) => ;
    //
    // public static virtual Result ProcessAndParse(out TAstNode? node, TLexer lexer, TPreprocessor? preprocessor = null)
    // {
    //     preprocessor ??= new TPreprocessor();
    //     var builder = new StringBuilder();
    //     var preprocessResult = preprocessor.EvaluateLexer(lexer, builder);
    //     lexer.ResetLexer(builder.ToString());
    //     return Result.Merge(preprocessResult, Parse(out node, lexer));
    // }
}
