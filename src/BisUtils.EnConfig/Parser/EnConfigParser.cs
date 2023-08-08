namespace BisUtils.EnConfig.Parser;

using Core.Parsing.Parser;
using Core.Parsing.Token.Matching;
using Core.Singleton;
using Lexer;
using Models;
using Tokens;

public interface IEnConfigParser : IBisParser<EnConfigFile, EnConfigLexer, EnConfigTokens>
{

}

public sealed class EnConfigParser : BisParser<EnConfigFile, EnConfigLexer, EnConfigTokens>, IEnConfigParser
{
    public static EnConfigParser Instance => BisSingletonProvider.LocateInstance<EnConfigParser>();


    public override void ParseToken(EnConfigFile file, EnConfigLexer lexer, BisTokenMatch match)
    {
        if (match.TokenType == EnConfigTokens.EnfusionIdentifier)
        {
         //TODO parse node
        }
    }
}
