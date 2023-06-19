namespace BisUtils.Core.Parsing;

using FResults;

public interface IBisPreProcessor
{
    Result ProcessLexer(BisMutableStringStepper lexer);
}
