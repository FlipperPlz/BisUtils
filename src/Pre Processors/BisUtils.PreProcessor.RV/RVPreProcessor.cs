namespace BisUtils.PreProcessor.RV;

using Core.Parsing;
using FResults;
using Models.Directives;

public interface IRVPreProcessor : IBisPreProcessor
{
    public List<IRVDefineDirective> MacroDefinitions { get; }
}

public class RVPreProcessor : IRVPreProcessor
{
    public List<IRVDefineDirective> MacroDefinitions { get; }

    public RVPreProcessor(List<IRVDefineDirective> macroDefinitions) => MacroDefinitions = macroDefinitions;

    public Result ProcessLexer(BisMutableStringStepper lexer)
    {
        throw new NotImplementedException();
    }

}
