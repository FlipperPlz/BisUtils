using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Statements;

namespace BisUtils.Parsers.ParamParser.Factories; 

public class RapStatementFactory {
    public static IRapStatement Create(Generated.ParamLang.ParamParser.StatementContext ctx) {
        if (ctx.classDeclaration() is { } @class) return (IRapStatement) RapClassDeclaration.FromContext(@class);
        if (ctx.externalClassDeclaration() is { } external) return RapExternalClassStatement.FromContext(external);
        if (ctx.tokenDeclaration() is { } var) return RapVariableDeclaration.FromContext(var);
        if (ctx.arrayAppension() is { } appension) return RapAppensionStatement.FromContext(appension);
        if (ctx.arrayDeclaration() is { } array) return  RapArrayDeclaration.FromContext(array);
        if (ctx.deleteStatement() is { } delete) return RapDeleteStatement.FromContext(delete);
        throw new NotSupportedException();
    }
}
