using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class RapDeleteStatement : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.DeleteStatementContext> {
    private string Target { get; set; } = null!;
    
    public string ToString(int indentation = char.MinValue) => new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
        .Append("delete ").Append(Target).Append(';').ToString();

    public RapDeleteStatement(string deleting) {
        Target = deleting;
    }

    private RapDeleteStatement() { }
    
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.DeleteStatementContext ctx) {
        if (ctx.identifier() is not { } identifier) throw new Exception("Nothing was given to delete.");
        Target = ctx.Start.InputStream.GetText(new Interval(identifier.Start.StartIndex, identifier.Stop.StopIndex));
        return this;
    }

    public static RapDeleteStatement FromContext(Generated.ParamLang.ParamParser.DeleteStatementContext ctx) =>
       (RapDeleteStatement) new RapDeleteStatement().ReadParseTree(ctx);
}