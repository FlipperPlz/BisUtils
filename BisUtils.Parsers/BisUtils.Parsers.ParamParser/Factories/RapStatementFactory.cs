using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Statements;

namespace BisUtils.Parsers.ParamParser.Factories; 

public class RapStatementFactory {
    public static IRapStatement Create(Generated.ParamLang.ParamParser.StatementContext ctx) {
        if (ctx.externalClassDeclaration() is { } external) return RapExternalClassStatement.FromContext(external);
        if (ctx.arrayAppension() is { } appension) return RapAppensionStatement.FromContext(appension);
        if (ctx.deleteStatement() is { } delete) return RapDeleteStatement.FromContext(delete);
        throw new NotSupportedException();
    }
}