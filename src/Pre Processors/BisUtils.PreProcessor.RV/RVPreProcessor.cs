namespace BisUtils.PreProcessor.RV;

using Core.Parsing;
using Core.Parsing.Lexer;
using Enumerations;
using Models.Directives;
using Utils;

public interface IRVPreProcessor : IBisPreProcessor<RvTypes>
{
    List<IRVDefineDirective> MacroDefinitions { get; }

    RVIncludeFinder IncludeLocator { get; }
}

public class RVPreProcessor : BisPreProcessor<RvTypes>, IRVPreProcessor
{
    public List<IRVDefineDirective> MacroDefinitions { get; } = new ();
    public required RVIncludeFinder IncludeLocator { get; init; }

    private static readonly IBisLexer<RvTypes>.TokenDefinition EOFDefinition =
        CreateTokenDefinition("rv.eof", RvTypes.EOF, 200);

    private static readonly IEnumerable<IBisLexer<RvTypes>.TokenDefinition> TokenDefinitions = new[]
    {
        EOFDefinition
    };

    public override IEnumerable<IBisLexer<RvTypes>.TokenDefinition> TokenTypes => TokenDefinitions;
    public override IBisLexer<RvTypes>.TokenDefinition EOFToken => EOFDefinition;

    public RVPreProcessor(string content, List<IRVDefineDirective> macroDefinitions) : base(content)
    {

    }

    protected override IBisLexer<RvTypes>.TokenMatch GetNextToken()
    {
        throw new NotImplementedException();
        throw new NotImplementedException();
    }

    private static IBisLexer<RvTypes>.TokenDefinition CreateTokenDefinition(string debugName, RvTypes tokenType, short tokenWeight) =>
        new() { DebugName = debugName, TokenId = tokenType, TokenWeight = tokenWeight };
}
