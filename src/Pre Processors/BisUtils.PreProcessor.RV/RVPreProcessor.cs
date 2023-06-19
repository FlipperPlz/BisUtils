namespace BisUtils.PreProcessor.RV;

using Core.Parsing;
using FResults;

public interface IRVPreProcessor : IBisPreProcessor
{

}

public class RVPreProcessor : IRVPreProcessor
{
    public Result ProcessLexer(BisMutableStringStepper lexer)
    {
        throw new NotImplementedException();
    }
}
