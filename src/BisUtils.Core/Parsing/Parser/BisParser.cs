﻿namespace BisUtils.Core.Parsing.Parser;

using Lexer;
using Microsoft.Extensions.Logging;
using Singleton;
using Token.Matching;
using Token.Typing;

public interface IBisParser<
    out TSyntaxTree,
    in TLexer,
    in TTokens,
    in TContextInfo
>
    where TLexer : IBisLexer<TTokens>
    where TTokens : BisTokenTypeSet<TTokens>, new()
    where TContextInfo : IBisParserContext, new()
{
    public TSyntaxTree Parse(TLexer lexer, ILogger? logger);

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

    public virtual TSyntaxTree Parse(TLexer lexer, ILogger? logger = default!)
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
            ParseToken(file, lexer, match, info, logger);
            mute = false;
        };

        while (info.ShouldContinue())
        {
            lexer.LexToken();
        }

        return file;
    }

    protected abstract void ParseToken(TSyntaxTree file, TLexer lexer, BisTokenMatch match, TContextInfo info, ILogger? logger);
}
