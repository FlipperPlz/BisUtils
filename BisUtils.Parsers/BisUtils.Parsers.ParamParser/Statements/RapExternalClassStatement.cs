using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class RapExternalClassStatement : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.ExternalClassDeclarationContext>, IComparable<RapExternalClassStatement> {
    public string Classname { get; set; }
    
    public string ToString(int indentation = char.MinValue) => new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
        .Append("class ").Append(Classname).Append(';').ToString();

    public RapExternalClassStatement(string classname) => Classname = classname;
    private RapExternalClassStatement() { }

    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.ExternalClassDeclarationContext ctx) {
        if (ctx.classname is not { } classname) throw new Exception();
        Classname = ctx.Start.InputStream.GetText(new Interval(classname.Start.StartIndex, classname.Stop.StopIndex));
        return this;
    }

    public static RapExternalClassStatement FromContext(Generated.ParamLang.ParamParser.ExternalClassDeclarationContext ctx) =>
        (RapExternalClassStatement) new RapExternalClassStatement().ReadParseTree(ctx);

    public int CompareTo(IRapStatement? other) {
        return other switch {
            RapClassDeclaration => -1,
            RapExternalClassStatement external => CompareTo(external),
            RapDeleteStatement => 1,
            RapAppensionStatement => 2,
            RapArrayDeclaration => 3,
            RapVariableDeclaration => 4, 
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(RapExternalClassStatement? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Classname, other.Classname, StringComparison.Ordinal);
    }
}