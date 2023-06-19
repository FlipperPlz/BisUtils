namespace BisUtils.Core.Parsing;

using FResults;

public interface IBisPreProcessor<in TLexer> where TLexer : BisMutableStringStepper
{
    Result ProcessLexer(TLexer lexer);
}

public interface IBisPreProcessor : IBisPreProcessor<BisMutableStringStepper>
{

}
