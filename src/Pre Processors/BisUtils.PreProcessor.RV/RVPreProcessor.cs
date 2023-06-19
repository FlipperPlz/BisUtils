namespace BisUtils.PreProcessor.RV;

using Core.Parsing;
using FResults;
using Models.Directives;
using Utils;

public interface IRVPreProcessor : IBisPreProcessor
{
    List<IRVDefineDirective> MacroDefinitions { get; }

    RVIncludeFinder IncludeLocator { get; }
}

public class RVPreProcessor : IRVPreProcessor
{
    public List<IRVDefineDirective> MacroDefinitions { get; }
    public RVIncludeFinder IncludeLocator { get; }

    public RVPreProcessor(List<IRVDefineDirective> macroDefinitions, RVIncludeFinder includeLocator)
    {
        MacroDefinitions = macroDefinitions;
        IncludeLocator = includeLocator;
    }

    public Result ProcessLexer(BisMutableStringStepper lexer)
    {
        throw new NotImplementedException();
    }

}
