namespace BisUtils.Core.Parsing.Processing;

using Lexer;
using Singleton;
using Token.Typing;

public interface IBisPreProcessor<TTokenTypes> where TTokenTypes : BisTokenTypeSet<TTokenTypes>, new()
{

    public void ProcessLexer(BisLexer<TTokenTypes> lexer);
}

public abstract class BisPreProcessor<TTokenTypes> : BisSingleton, IBisPreProcessor<TTokenTypes> where TTokenTypes : BisTokenTypeSet<TTokenTypes>, new()
{
    // ReSharper disable once PublicConstructorInAbstractClass
    public BisPreProcessor()
    {

    }

    public abstract void ProcessLexer(BisLexer<TTokenTypes> lexer);
}
