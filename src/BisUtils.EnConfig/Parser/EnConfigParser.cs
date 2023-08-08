namespace BisUtils.EnConfig.Parser;

using Context;
using Core.Parsing.Parser;
using Core.Parsing.Token.Matching;
using Core.Singleton;
using Lexer;
using Models;
using Tokens;

public interface IEnConfigParser : IBisParser<EnConfigFile, EnConfigLexer, EnConfigTokens, EnConfigContext>
{

}

public sealed class EnConfigParser : BisParser<EnConfigFile, EnConfigLexer, EnConfigTokens, EnConfigContext>, IEnConfigParser
{
    public static EnConfigParser Instance => BisSingletonProvider.LocateInstance<EnConfigParser>();

    public override void ParseToken(EnConfigFile file, EnConfigLexer lexer, BisTokenMatch match, EnConfigContext info)
    {
        if (match.TokenType == EnConfigTokens.EnfusionIdentifier)
        {
            //TODO parse node
        }
    }
}
