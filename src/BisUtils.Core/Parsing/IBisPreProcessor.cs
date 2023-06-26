namespace BisUtils.Core.Parsing;

using Lexer;

public interface IBisPreProcessor<TPreProcTypes> : IBisLexer<TPreProcTypes> where TPreProcTypes : Enum
{
}

public abstract class BisPreProcessor<TPreProcTypes> : BisLexer<TPreProcTypes>, IBisPreProcessor<TPreProcTypes> where TPreProcTypes : Enum
{
    public override IBisLexer<TPreProcTypes>.TokenDefinition? ErrorToken => null;
    protected BisPreProcessor(string content) : base(content) => TokenizeUntilEnd();
}
