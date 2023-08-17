namespace BisUtils.Core.ParsingFramework.Parser;

using Extensions;
using Lexer;
using Microsoft.Extensions.Logging;
using Singleton;
using Tokens.Match;
using Tokens.TypeSet;

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

    public virtual TSyntaxTree Parse(TLexer lexer, ILogger? logger = default!)
    {
        var file = new TSyntaxTree();
        var info = new TContextInfo();
        var mute = false;

        lexer.OnTokenMatched += delegate(ref BisTokenMatch match)
        {
            if (mute || !CanTokenBeMatched(info, ref match, lexer, logger))
            {
                return;
            }

            mute = true;
            ParseToken(file, lexer, ref match, info, logger);
            mute = false;
        };

        while (info.ShouldContinue())
        {
            lexer.LexToken();
        }

        return file;
    }

    protected virtual bool CanTokenBeMatched(TContextInfo info, ref BisTokenMatch match, TLexer bisLexer, ILogger? logger)
        => false;


    protected abstract void ParseToken(TSyntaxTree file, TLexer lexer, ref BisTokenMatch match, TContextInfo info, ILogger? logger);
}
