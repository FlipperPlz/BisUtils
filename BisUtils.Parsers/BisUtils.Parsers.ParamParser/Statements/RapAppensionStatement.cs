using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class RapAppensionStatement : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.ArrayAppensionContext> {
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

}