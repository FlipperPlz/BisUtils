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

    protected override void ParseToken(ParamFile file, RvConfigLexer lexer, BisTokenMatch match, RvConfigParseContext info)
    {
        if (match.TokenType == RvConfigTokenSet.ConfigWhitespace || match.TokenType == RvConfigTokenSet.RvNewLine)
        {
            return;
        }

        if (match.TokenType == RvConfigTokenSet.BisEOF)
        {
            if (info.Context.Count > 1)
            {
                throw new NotSupportedException(); //TODO: Error
            }

            info.ShouldEnd = true;
            return;
        }

        if (match.TokenType == RvConfigTokenSet.ConfigRCurly)
        {
            if ((match = lexer.LexToken()).TokenType != RvConfigTokenSet.ConfigSeparator)
            {
                throw new NotSupportedException(); //TODO: Error
            }

            if (info.Context.Count == 1)
            {
                throw new NotSupportedException(); //TODO: Error no class to end
            }

            info.Context.Pop();
            return;
        }


        if (match.TokenType == RvConfigTokenSet.ConfigClass)
        {
            //TODO Class
        }



    }
}
