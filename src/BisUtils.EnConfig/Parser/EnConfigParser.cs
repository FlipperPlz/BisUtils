namespace BisUtils.EnConfig.Parser;

using Context;
using Core.Parsing.Parser;
using Core.Parsing.Token.Matching;
using Core.Singleton;
using Lexer;
using Models;
using Tokens;

public interface IEnConfigParser : IBisParser<EnConfigFile, EnConfigLexer, EnConfigTokenSet, EnConfigContext>
{

}

public sealed class EnConfigParser : BisParser<EnConfigFile, EnConfigLexer, EnConfigTokenSet, EnConfigContext>, IEnConfigParser
{
    public static EnConfigParser Instance => BisSingletonProvider.LocateInstance<EnConfigParser>();

    protected override void ParseToken(EnConfigFile file, EnConfigLexer lexer, BisTokenMatch match, EnConfigContext info)
    {
        if (match.TokenType == EnConfigTokenSet.EnfusionIdentifier)
        {
            //TODO parse node
        }
    }
}
