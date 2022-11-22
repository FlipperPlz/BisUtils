using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class RapDeleteStatement : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.DeleteStatementContext>, IComparable<RapDeleteStatement> {
    public string Target { get; set; } = null!;
    
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

    public int CompareTo(IRapStatement? other) {
        return other switch {
            RapClassDeclaration => -2,
            RapExternalClassStatement => -1,
            RapDeleteStatement delete => CompareTo(delete),
            RapAppensionStatement => 1,
            RapArrayDeclaration => 2,
            RapVariableDeclaration => 3, 
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(RapDeleteStatement? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Target, other.Target, StringComparison.Ordinal);
    }
}