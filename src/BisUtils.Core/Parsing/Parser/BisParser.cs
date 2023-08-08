namespace BisUtils.Core.Parsing.Parser;

using Lexer;using Singleton;
using Token.Matching;
using Token.Typing;

public interface IBisParser<
    TSyntaxTree,
    in TLexer,
    in TTokens
>
    where TLexer : BisLexer<TTokens>
    where TTokens : BisTokenTypeSet<TTokens>, new()
{
    public TSyntaxTree Parse(TLexer lexer);

    public void ParseToken(TSyntaxTree file, TLexer lexer, IBisTokenMatch match);
}

public abstract class BisParser<
    TSyntaxTree,
    TLexer,
    TTokens
> : BisSingleton, IBisParser<TSyntaxTree, TLexer, TTokens>
    where TLexer : BisLexer<TTokens>
    where TTokens : BisTokenTypeSet<TTokens>, new()
    where TSyntaxTree : new()
{

    public TSyntaxTree Parse(TLexer lexer)
    {
        var file = new TSyntaxTree();

        lexer.OnTokenMatched += (_, match) =>
        {
            lexer.MuteEvents = true;
            ParseToken(file, lexer, match);
            lexer.MuteEvents = false;
        };

        return file;
    }

    public abstract void ParseToken(TSyntaxTree file, TLexer lexer, IBisTokenMatch match);
}
