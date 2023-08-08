namespace BisUtils.Core.Parsing.Parser;

using Lexer;
using Token.Typing;

public interface IBisParser<
    out TSyntaxTree,
    in TLexer,
    in TTokens
>
    where TLexer : BisLexer<TTokens>
    where TTokens : BisTokenTypeSet<TTokens>, new()
{
    public TSyntaxTree Parse(TLexer lexer);
}

public abstract class BisParser<
    TSyntaxTree,
    TLexer,
    TTokens
> : IBisParser<TSyntaxTree, TLexer, TTokens>
    where TLexer : BisLexer<TTokens>
    where TTokens : BisTokenTypeSet<TTokens>, new()
{
    // ReSharper disable once PublicConstructorInAbstractClass
    public BisParser()
    {

    }

    public abstract TSyntaxTree Parse(TLexer lexer);
}
