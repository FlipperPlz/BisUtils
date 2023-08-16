namespace BisUtils.Core.ParsingFramework.Parser;

using Lexer;
using Microsoft.Extensions.Logging;
using Tokens.TypeSet;

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
