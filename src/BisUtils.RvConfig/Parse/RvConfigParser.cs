namespace BisUtils.RvConfig.Parse;

using Core.Extensions;
using Core.Parsing.Parser;
using Core.Parsing.Token.Matching;
using Core.Parsing.Token.Typing;
using Core.Singleton;
using Lexer;
using Microsoft.Extensions.Logging;
using Models;
using Models.Statements;
using Models.Stubs;
using Models.Stubs.Holders;
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

    protected override void ParseToken(ParamFile file, RvConfigLexer lexer, BisTokenMatch match, RvConfigParseContext info, ILogger? logger)
    {
        var context = info.CurrentContext;
        if
        (
            !match.IfMatches(_ => { }, RvConfigTokenSet.ConfigWhitespace) &&
            !match.IfMatches(_ => { }, RvConfigTokenSet.RvNewLine) &&
            !match.IfMatches(_ => ParseEOF(info, logger), RvConfigTokenSet.BisEOF) &&
            !match.IfMatches(_ => ParseRCurly(lexer, out match, info, logger), RvConfigTokenSet.ConfigRCurly) &&
            !match.IfMatches(_ => ParseClass(context, file, lexer, out match, info, logger), RvConfigTokenSet.ConfigClass) &&
            !match.IfMatches(_ => ParseDelete(context, file, lexer, out match, logger), RvConfigTokenSet.ConfigDelete)

        )
        {
            throw new NotSupportedException(); //TODO: Error: your token currently makes no sense to me

        }
    }

    private static void ParseDelete(IParamStatementHolder context, IParamFile file, RvConfigLexer lexer, out BisTokenMatch match, ILogger? logger)
    {
        AssertWhitespace(lexer, out match, logger);
        AssertToken(match, RvConfigTokenSet.RvIdentifier, logger);
        context.Statements.Add(new ParamDelete(match.TokenText, file, context, logger));
    }

    private static void ParseClass(IParamStatementHolder context, IParamFile file, RvConfigLexer lexer,
        out BisTokenMatch match, RvConfigParseContext info, ILogger? logger)
    {
        string? superClass = null;

        AssertWhitespace(lexer, out match, logger);
        AssertToken(match, RvConfigTokenSet.RvIdentifier, logger);
        var classname = match.TokenText;
        _ = lexer.LexWhitespace(ref match);
        var nextToken = match;
        if
        (
            !nextToken.IfMatches(_ => ParseExternalClass(classname, file, context, logger), RvConfigTokenSet.ConfigSeparator) &&
            !nextToken.IfMatches(_ => ParseSuperClassname(out superClass, ref nextToken, lexer, logger), RvConfigTokenSet.ConfigColon) &&
            !nextToken.IfMatches(_ => ParseClassBody(classname, superClass, ref nextToken, file, lexer, info, logger), RvConfigTokenSet.ConfigRCurly)
        )
        {
            throw new NotSupportedException(); //TODO: Error: token not matched
        }
        match = nextToken;
    }

    private static void ParseClassBody(string classname, string? superClass, ref BisTokenMatch nextToken,
        IParamFile file, RvConfigLexer lexer, RvConfigParseContext info, ILogger? logger) =>
        info.CurrentContext.Statements.Add(new ParamClass(classname, superClass, new List<IParamStatement>(), file, info.CurrentContext, logger));

    private static void ParseSuperClassname(out string? superClass, ref BisTokenMatch nextToken,
        RvConfigLexer lexer, ILogger? logger)
    {
        _ = lexer.LexWhitespace(ref nextToken, false);
        superClass = nextToken.TokenText;
    }

    private static void ParseExternalClass(string classname, IParamFile file, IParamStatementHolder context, ILogger? logger) =>
        context.Statements.Add(new ParamExternalClass(classname, file, context, logger));

    private static void AssertWhitespace(RvConfigLexer lexer, out BisTokenMatch match, ILogger? logger)
    {
        if (lexer.LexWhitespace(ref match) < 1)
        {
            throw new NotSupportedException(); //TODO: Error: Whitespace not found
        }
    }

    private static void AssertToken(IBisTokenMatch match, IBisTokenType validType, ILogger? logger)
    {
        if (match.TokenType != validType)
        {
            throw new NotSupportedException(); //TODO: Error: token not matched
        }
    }

    private static void ParseEOF(RvConfigParseContext info, ILogger? logger = default)
    {
        if (info.Context.Count > 1)
        {
            throw new NotSupportedException(); //TODO: Error
        }

        info.ShouldEnd = true;
    }

    private static void ParseRCurly(RvConfigLexer lexer, out BisTokenMatch match, RvConfigParseContext info,
        ILogger? logger)
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
    }
}
