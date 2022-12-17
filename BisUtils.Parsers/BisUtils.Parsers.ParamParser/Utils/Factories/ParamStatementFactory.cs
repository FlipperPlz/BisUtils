using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Statements;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Utils.Factories;

public static class ParamStatementFactory {
    public static ParamStatement Create(Generated.ParamLang.ParamParser.StatementContext ctx) {
        if (ctx.classDeclaration() is { } @class) return new ParamClassDeclaration(@class);
        if (ctx.externalClassDeclaration() is { } external) return new ParamExternalClassStatement(external);
        if (ctx.tokenDeclaration() is { } var) return new ParamVariableDeclaration(var);
        if (ctx.arrayAppension() is { } appension) return new ParamAppensionStatement(appension);
        if (ctx.arrayDeclaration() is { } array) return new ParamArrayDeclaration(array);
        if (ctx.deleteStatement() is { } delete) return new ParamDeleteStatement(delete);
        throw new NotSupportedException();
    }

    public static IBisSerializable<ParamSerializationOptions> CreateSerializable(ParamStatement statement) {
        return statement switch {
            ParamDeleteStatement deleteStatement => deleteStatement,
            ParamExternalClassStatement externalClassStatement => externalClassStatement,
            ParamVariableDeclaration variableDeclaration => variableDeclaration,
            ParamAppensionStatement appensionStatement => appensionStatement,
            ParamClassDeclaration classDeclaration => classDeclaration,
            ParamArrayDeclaration arrayDeclaration => arrayDeclaration,
            _ => throw new Exception()
        };
    }
    
    public static IBisBinarizable CreateBinarizable(ParamStatement statement) {
        return statement switch {
            ParamDeleteStatement deleteStatement => deleteStatement,
            ParamExternalClassStatement externalClassStatement => externalClassStatement,
            ParamVariableDeclaration variableDeclaration => variableDeclaration,
            ParamAppensionStatement appensionStatement => appensionStatement,
            ParamClassDeclaration classDeclaration => classDeclaration,
            ParamArrayDeclaration arrayDeclaration => arrayDeclaration,
            _ => throw new Exception()
        };
    }
}