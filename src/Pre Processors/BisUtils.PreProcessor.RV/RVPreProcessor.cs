namespace BisUtils.PreProcessor.RV;

using Core.Parsing;
using Core.Parsing.Lexer;
using FResults;
using Lexer;
using Models.Directives;
using Utils;

public interface IRVPreProcessor : IBisPreProcessor<RVLexer>
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

    public Result ProcessLexer(RVLexer lexer) => throw new NotImplementedException();
}
