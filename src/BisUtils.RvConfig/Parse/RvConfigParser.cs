namespace BisUtils.RvConfig.Parse;

using Core.Extensions;
using Core.Parsing.Parser;
using Core.Parsing.Token.Matching;
using Core.Parsing.Token.Typing;
using Core.Singleton;
using Enumerations;
using Lexer;
using Microsoft.Extensions.Logging;
using Models;
using Models.Statements;
using Models.Stubs;
using Models.Stubs.Holders;
using Tokens;

public interface IRvConfigParser : IBisParser<
    RvConfigFile,
    RvConfigLexer,
    RvConfigTokenSet,
    RvConfigParseContext
>
{

    public static void ParseVariable(IParamClass context, RvConfigFile file, RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger)
    {
        var variableName = match.TokenText;
        var variableType = ParseVariableType(lexer, ref match, logger);
        var op = ParseVariableOperator(lexer, ref match, logger);


        throw new NotImplementedException();
    }


    public static RvConfigValueType ParseVariableType(RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger)
    {
        lexer.LexWhitespace(ref match);
        if (!match.IsType(RvConfigTokenSet.ConfigLSquare))
        {
            return RvConfigValueType.ParamString;
        }

        ParseArraySquare(lexer, ref match, logger);
        return RvConfigValueType.ParamArray;
    }

    public static ParamOperatorType ParseVariableOperator(RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger)
    {
        lexer.LexWhitespace(ref match);
        if (match.IsType(RvConfigTokenSet.ConfigAssign))
        {
            return ParamOperatorType.Assign;
        }

        if (match.IsType(RvConfigTokenSet.ConfigAddAssign))
        {
            return ParamOperatorType.AddAssign;
        }

        if (match.IsType(RvConfigTokenSet.ConfigSubAssign))
        {
            return ParamOperatorType.SubAssign;
        }

        throw new NotImplementedException(); //TODO: Unknown Operator
    }

    public static void ParseArraySquare(RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger)
    {
        lexer.LexWhitespace(ref match);
        if (match.IsNotType(RvConfigTokenSet.ConfigRSquare))
        {
        }
    }

    public static void ParseEnum(IParamClass context, RvConfigFile file, RvConfigLexer lexer, out BisTokenMatch match, RvConfigParseContext info, ILogger? logger)
    {
        AssertToken(match = lexer.LexToken(), RvConfigTokenSet.ConfigLCurly, logger);

        //TODO: Enum values should be stored inside info?
        throw new NotImplementedException();
    }

    public static void ParseDelete(IParamStatementHolder context, IRvConfigFile file, RvConfigLexer lexer, out BisTokenMatch match, ILogger? logger)
    {
        AssertWhitespace(lexer, out match, logger);
        AssertToken(match, RvConfigTokenSet.RvIdentifier, logger);
        context.Statements.Add(new ParamDelete(match.TokenText, file, context, logger));
    }

    public static void ParseClass(IParamStatementHolder context, IRvConfigFile file, RvConfigLexer lexer,
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
            !nextToken.IfMatches(RvConfigTokenSet.ConfigSeparator, _ => ParseExternalClass(classname, file, context, logger)) &&
            !nextToken.IfMatches(RvConfigTokenSet.ConfigColon,_ => ParseSuperClassname(out superClass, ref nextToken, lexer, logger)) &&
            !nextToken.IfMatches(RvConfigTokenSet.ConfigRCurly, _ => ParseClassBody(classname, superClass, file, info, logger))
        )
        {
            throw new NotSupportedException(); //TODO: Error: token not matched
        }
        match = nextToken;
    }

    public static void ParseClassBody(string classname, string? superClass,
        IRvConfigFile file, RvConfigParseContext info, ILogger? logger) =>
        info.CurrentContext.Statements.Add(new ParamClass(classname, superClass, new List<IParamStatement>(), file, info.CurrentContext, logger));

    public static void ParseSuperClassname(out string? superClass, ref BisTokenMatch nextToken, RvConfigLexer lexer, ILogger? logger)
    {
        _ = lexer.LexWhitespace(ref nextToken, false);
        superClass = nextToken.TokenText;
    }

    public static void ParseExternalClass(string classname, IRvConfigFile file, IParamStatementHolder context, ILogger? logger) =>
        context.Statements.Add(new ParamExternalClass(classname, file, context, logger));

    public static void AssertWhitespace(RvConfigLexer lexer, out BisTokenMatch match, ILogger? logger)
    {
        if (lexer.LexWhitespace(ref match) < 1)
        {
            throw new NotSupportedException(); //TODO: Error: Whitespace not found
        }
    }

    public static void AssertToken(IBisTokenMatch match, IBisTokenType validType, ILogger? logger)
    {
        if (match.TokenType != validType)
        {
            throw new NotSupportedException(); //TODO: Error: token not matched
        }
    }

    public static void ParseEOF(RvConfigParseContext info, ILogger? logger = default)
    {
        if (info.Context.Count > 1)
        {
            throw new NotSupportedException(); //TODO: Error
        }

        info.ShouldEnd = true;
    }

    public static void ParseRCurly(RvConfigLexer lexer, out BisTokenMatch match, RvConfigParseContext info, ILogger? logger)
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

public class RvConfigParser : BisParser<
    RvConfigFile,
    RvConfigLexer,
    RvConfigTokenSet,
    RvConfigParseContext
>, IRvConfigParser
{
    public static readonly RvConfigParser Instance = BisSingletonProvider.LocateInstance<RvConfigParser>();

    protected override void ParseToken(RvConfigFile file, RvConfigLexer lexer, BisTokenMatch match, RvConfigParseContext info, ILogger? logger)
    {
        var context = info.CurrentContext;
        if
        (
            !match.IfMatches(RvConfigTokenSet.RvWhitespace, _ => { }) &&
            !match.IfMatches(RvConfigTokenSet.RvNewLine, _ => { }) &&
            !match.IfMatches(RvConfigTokenSet.BisEOF, _ => IRvConfigParser.ParseEOF(info, logger)) &&
            !match.IfMatches(RvConfigTokenSet.ConfigRCurly, _ => IRvConfigParser.ParseRCurly(lexer, out match, info, logger)) &&
            !match.IfMatches(RvConfigTokenSet.ConfigClass, _ => IRvConfigParser.ParseClass(context, file, lexer, out match, info, logger)) &&
            !match.IfMatches(RvConfigTokenSet.ConfigDelete, _ => IRvConfigParser.ParseDelete(context, file, lexer, out match, logger)) &&
            !match.IfMatches(RvConfigTokenSet.ConfigDelete, _ => IRvConfigParser.ParseEnum(context, file, lexer, out match, info, logger)) &&
            !match.IfMatches(RvConfigTokenSet.RvIdentifier, _ => IRvConfigParser.ParseVariable(context, file, lexer, ref match, logger))
        )
        {
            throw new NotSupportedException(); //TODO: Error: your token currently makes no sense to me

        }
    }

}
