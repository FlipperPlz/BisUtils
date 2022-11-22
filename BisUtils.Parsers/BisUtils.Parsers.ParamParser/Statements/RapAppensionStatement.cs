using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class RapAppensionStatement : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.ArrayAppensionContext>, IComparable<RapAppensionStatement>{
    public string Target { get; set; } = string.Empty;
    public RapArray Array { get; set; } = RapArray.EmptyArray;

    public RapAppensionStatement(string arrayName, RapArray array) {
        Target = arrayName;
        Array = array;
    }
    
    private RapAppensionStatement() {}
    
    public string ToString(int indentation = 0) => 
        new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
            .Append(Target).Append("[] += ").Append(Array.ToString()).Append(';').ToString();

    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.ArrayAppensionContext ctx) {
        if (ctx.arrayName() is not { } arrayNameCtx) throw new Exception();
        if (ctx.literalArray() is not { } literalArrayCtx) throw new Exception();
        Target = ctx.Start.InputStream.GetText(new Interval(arrayNameCtx.identifier().Start.StartIndex, arrayNameCtx.identifier().Stop.StopIndex));
        Array.ReadParseTree(literalArrayCtx);
        return this;
    }

    public static RapAppensionStatement FromContext(Generated.ParamLang.ParamParser.ArrayAppensionContext ctx) =>
        (RapAppensionStatement) new RapAppensionStatement().ReadParseTree(ctx);

    public int CompareTo(IRapStatement? other) {
        return other switch {
            RapClassDeclaration => -3,
            RapExternalClassStatement => -2,
            RapDeleteStatement => -1,
            RapAppensionStatement append => CompareTo(append),
            RapArrayDeclaration => 1,
            RapVariableDeclaration => 2, 
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(RapAppensionStatement? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Target, other.Target, StringComparison.Ordinal);
    }
}