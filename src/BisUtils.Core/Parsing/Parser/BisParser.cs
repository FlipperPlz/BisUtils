namespace BisUtils.Core.Parsing.Parser;

using Lexer;
using Singleton;
using Token.Matching;
using Token.Typing;

public interface IBisParser<
    TSyntaxTree,
    in TLexer,
    in TTokens,
    in TContextInfo
>
    where TLexer : IBisLexer<TTokens>
    where TTokens : BisTokenTypeSet<TTokens>, new()
    where TSyntaxTree : new()
    where TContextInfo : IBisParserContext, new()
{
    public TSyntaxTree Parse(TLexer lexer);

    public void ParseToken(TSyntaxTree file, TLexer lexer, BisTokenMatch match, TContextInfo info);
}

public abstract class BisParser<
    TSyntaxTree,
    TLexer,
    TTokens,
    TContextInfo
> : BisSingleton, IBisParser<TSyntaxTree, TLexer, TTokens, TContextInfo>
    where TLexer : IBisLexer<TTokens>
    where TTokens : BisTokenTypeSet<TTokens>, new()
    where TSyntaxTree : new()
    where TContextInfo : IBisParserContext, new()
{

    public virtual TSyntaxTree Parse(TLexer lexer)
    {
        var file = new TSyntaxTree();
        var info = new TContextInfo();
        var mute = false;

        lexer.OnTokenMatched += (_, match) =>
        {
            if (mute)
            {
                return;
            }

            mute = true;
            ParseToken(file, lexer, match, info);
            mute = false;
        };

        while (info.ShouldContinue())
        {
            lexer.LexToken();
        }

        return file;
    }

    public abstract void ParseToken(TSyntaxTree file, TLexer lexer, BisTokenMatch match, TContextInfo info);
}
