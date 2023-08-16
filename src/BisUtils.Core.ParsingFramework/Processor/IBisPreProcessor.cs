namespace BisUtils.Core.ParsingFramework.Processor;

using Lexer;
using Microsoft.Extensions.Logging;
using Tokens.TypeSet;

public interface IBisPreProcessor<TTokenTypes> where TTokenTypes : BisTokenTypeSet<TTokenTypes>, new()
{

    public void ProcessLexer(BisLexer<TTokenTypes> lexer, ILogger? logger);
}
