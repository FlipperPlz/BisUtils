namespace BisUtils.RvConfig.Parse;

using Core.Parsing.Parser;
using Core.Parsing.Token.Matching;
using Core.Singleton;
using Lexer;
using Models;
using Tokens;

public interface IRvConfigParser : IBisParser<
    ParamFile,
    RvConfigLexer,
    RvConfigTokenSet,
    RvConfigParseContext
>
{
}

public class RvConfigParser : BisParser<
    ParamFile,
    RvConfigLexer,
    RvConfigTokenSet,
    RvConfigParseContext
>, IRvConfigParser
{
    public static readonly RvConfigParser Instance = BisSingletonProvider.LocateInstance<RvConfigParser>();

    public override void ParseToken(ParamFile file, RvConfigLexer lexer, BisTokenMatch match, RvConfigParseContext info)
    {
        switch (match.TokenType)
        {

        }
    }
}
