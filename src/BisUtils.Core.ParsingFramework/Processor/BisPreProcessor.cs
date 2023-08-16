namespace BisUtils.Core.ParsingFramework.Processor;

using Lexer;
using Microsoft.Extensions.Logging;
using Singleton;
using Tokens.TypeSet;

public abstract class BisPreProcessor<TTokenTypes> : BisSingleton, IBisPreProcessor<TTokenTypes> where TTokenTypes : BisTokenTypeSet<TTokenTypes>, new()
{
    // ReSharper disable once PublicConstructorInAbstractClass
    protected BisPreProcessor()
    {

    }

    public abstract void ProcessLexer(BisLexer<TTokenTypes> lexer, ILogger? logger);
}
