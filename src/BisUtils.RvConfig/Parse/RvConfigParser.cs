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
    void ParseClassBody(string classname, string? superClass, IRvConfigFile file, RvConfigParseContext info,
        ILogger? logger);
    void ParseArraySquare(RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger);
    void ParseVariable(IParamClass context, RvConfigFile file, RvConfigLexer lexer, ref BisTokenMatch match,
        ILogger? logger);

    void ParseRCurly(RvConfigLexer lexer, out BisTokenMatch match, RvConfigParseContext info, ILogger? logger);
    void ParseEOF(RvConfigParseContext info, ILogger? logger = default);
    void AssertToken(IBisTokenMatch match, IBisTokenType validType, ILogger? logger);
    void ParseExternalClass(string classname, IRvConfigFile file, IParamStatementHolder context, ILogger? logger);
    void ParseSuperClassname(out string? superClass, ref BisTokenMatch nextToken, RvConfigLexer lexer, ILogger? logger);

    RvConfigValueType ParseVariableType(RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger);
    ParamOperatorType ParseVariableOperator(RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger);


    void AssertWhitespace(RvConfigLexer lexer, out BisTokenMatch match, ILogger? logger);
}

public sealed class RvConfigParser : BisParser<
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
            !match.IfMatches(RvConfigTokenSet.BisEOF, _ => ParseEOF(info, logger)) &&
            !match.IfMatches(RvConfigTokenSet.ConfigRCurly, _ => ParseRCurly(lexer, out match, info, logger)) &&
            !match.IfMatches(RvConfigTokenSet.ConfigClass, _ => ParseClass(context, file, lexer, out match, info, logger)) &&
            !match.IfMatches(RvConfigTokenSet.ConfigDelete, _ => ParseDelete(context, file, lexer, out match, logger)) &&
            !match.IfMatches(RvConfigTokenSet.ConfigDelete, _ => ParseEnum(context, file, lexer, out match, info, logger)) &&
            !match.IfMatches(RvConfigTokenSet.RvIdentifier, _ => ParseVariable(context, file, lexer, ref match, logger))
        )
        {
            throw new NotSupportedException(); //TODO: Error: your token currently makes no sense to me

        }
    }

    public void ParseVariable(IParamClass context, RvConfigFile file, RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger)
    {
        // var variableName = match.TokenText;
        // var variableType = ParseVariableType(lexer, ref match, logger);
        // var op = ParseVariableOperator(lexer, ref match, logger);


        throw new NotImplementedException();
    }


    public RvConfigValueType ParseVariableType(RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger)
    {
        lexer.LexWhitespace(ref match);
        if (!match.IsType(RvConfigTokenSet.ConfigLSquare))
        {
            return RvConfigValueType.ParamString;
        }

        ParseArraySquare(lexer, ref match, logger);
        return RvConfigValueType.ParamArray;
    }

    public ParamOperatorType ParseVariableOperator(RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger)
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

    public void ParseArraySquare(RvConfigLexer lexer, ref BisTokenMatch match, ILogger? logger)
    {
        lexer.LexWhitespace(ref match);
        if (match.IsNotType(RvConfigTokenSet.ConfigRSquare))
        {
        }
    }

    public void ParseEnum(IParamClass context, RvConfigFile file, RvConfigLexer lexer, out BisTokenMatch match, RvConfigParseContext info, ILogger? logger)
    {
        AssertToken(match = lexer.LexToken(), RvConfigTokenSet.ConfigLCurly, logger);

        //TODO: Enum values should be stored inside info?
        throw new NotImplementedException();
    }

    public void ParseDelete(IParamStatementHolder context, IRvConfigFile file, RvConfigLexer lexer, out BisTokenMatch match, ILogger? logger)
    {
        AssertWhitespace(lexer, out match, logger);
        AssertToken(match, RvConfigTokenSet.RvIdentifier, logger);
        context.Statements.Add(new ParamDelete(match.TokenText, file, context, logger));
    }

    public void ParseClass(IParamStatementHolder context, IRvConfigFile file, RvConfigLexer lexer,
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

    public void ParseClassBody(string classname, string? superClass, IRvConfigFile file, RvConfigParseContext info, ILogger? logger) =>
        info.CurrentContext.Statements.Add(new ParamClass(classname, superClass, new List<IParamStatement>(), file, info.CurrentContext, logger));

    public void ParseSuperClassname(out string? superClass, ref BisTokenMatch nextToken, RvConfigLexer lexer, ILogger? logger)
    {
        _ = lexer.LexWhitespace(ref nextToken, false);
        superClass = nextToken.TokenText;
    }

    public void ParseExternalClass(string classname, IRvConfigFile file, IParamStatementHolder context, ILogger? logger) =>
        context.Statements.Add(new ParamExternalClass(classname, file, context, logger));


    public void AssertWhitespace(RvConfigLexer lexer, out BisTokenMatch match, ILogger? logger)
    {
        if (lexer.LexWhitespace(ref match) < 1)
        {
            throw new NotSupportedException(); //TODO: Error: Whitespace not found
        }
    }

    public void AssertToken(IBisTokenMatch match, IBisTokenType validType, ILogger? logger)
    {
        if (match.TokenType != validType)
        {
            throw new NotSupportedException(); //TODO: Error: token not matched
        }
    }

    public void ParseEOF(RvConfigParseContext info, ILogger? logger = default)
    {
        if (info.Context.Count > 1)
        {
            throw new NotSupportedException(); //TODO: Error
        }

        info.ShouldEnd = true;
    }

    public void ParseRCurly(RvConfigLexer lexer, out BisTokenMatch match, RvConfigParseContext info, ILogger? logger)
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
