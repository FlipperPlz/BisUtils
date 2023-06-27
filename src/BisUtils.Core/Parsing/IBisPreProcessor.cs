namespace BisUtils.Core.Parsing;

using System.Text;
using FResults;
using Lexer;

public interface IBisPreProcessor<TPreProcTypes> : IBisLexer<TPreProcTypes> where TPreProcTypes : Enum
{
    public Result PreProcessLexer(StringBuilder? builder);
}

public abstract class BisPreProcessor<TPreProcTypes> : BisLexer<TPreProcTypes>, IBisPreProcessor<TPreProcTypes> where TPreProcTypes : Enum
{
    public override IBisLexer<TPreProcTypes>.TokenDefinition? ErrorToken => null;

    protected BisPreProcessor(string content) : base(content)
    {

    }

    public abstract Result PreProcessLexer(StringBuilder? builder);
}
