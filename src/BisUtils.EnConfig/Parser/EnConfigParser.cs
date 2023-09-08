namespace BisUtils.EnConfig.Parser;

using Context;
using Core.Singleton;
using LangAssembler.Lexer.Models.Match;
using LangAssembler.Parser;
using Lexer;
using Microsoft.Extensions.Logging;
using Models;
using Tokens;

public interface IEnConfigParser : IParser<EnConfigFile, EnConfigLexer>
{

}

public sealed class EnConfigParser : Parser<EnConfigFile, EnConfigLexer, EnConfigContext>, IEnConfigParser
{
    public static EnConfigParser Instance => new EnConfigParser();

    private EnConfigParser()
    {

    }

    protected override void ParseToken(EnConfigFile root, EnConfigLexer lexer, ref ITokenMatch match, EnConfigContext info, ILogger? logger)
        => throw new NotImplementedException();
}
