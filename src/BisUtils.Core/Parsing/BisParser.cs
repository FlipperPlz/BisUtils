namespace BisUtils.Core.Parsing;

using System.Text;
using FResults;
using Lexer;

#pragma warning disable CA1000
public interface IBisParser<TAstNode, in TLexer, TTypes> where TLexer : BisLexer<TTypes> where TTypes : Enum
{
    public static virtual Result Parse(out TAstNode? node, TLexer lexer) => throw new NotSupportedException();

}

public interface IBisParser<TAstNode, in TLexer, TTypes, in TPreprocessor> : IBisParser<TAstNode, TLexer, TTypes> where TLexer : BisLexer<TTypes> where TPreprocessor : BisPreProcessorBase, new() where TTypes : Enum
{
    public static virtual Result ProcessAndParse
    (
        out TAstNode? node,
        TLexer lexer,
        TPreprocessor? preprocessor = null
    )
    {
        var builder = new StringBuilder();
        preprocessor ??= new TPreprocessor();
        preprocessor.EvaluateLexer(lexer, builder);
        lexer.ResetLexer(builder.ToString());
        throw new NotSupportedException();
        //return Parse(out node, lexer);
    }
}
