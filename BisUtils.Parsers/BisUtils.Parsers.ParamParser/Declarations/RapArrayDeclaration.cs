using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;

namespace BisUtils.Parsers.ParamParser.Declarations; 

public class RapArrayDeclaration :  IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.ArrayDeclarationContext> {
    public string ArrayName { get; set; } = string.Empty;
    public RapArray ArrayValue { get; set; } = RapArray.EmptyArray;
    public string ToString(int indentation = char.MinValue) => new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
        .Append(ArrayName).Append("[] = ").Append(ArrayValue.ToString()).Append(';').ToString();

    public RapArrayDeclaration(string arrayName, RapArray array) {
        ArrayName = arrayName;
        ArrayValue = array;
    }
    
    private RapArrayDeclaration() {}
    
    public static RapArrayDeclaration FromContext(Generated.ParamLang.ParamParser.ArrayDeclarationContext ctx) =>
        (RapArrayDeclaration) new RapArrayDeclaration().ReadParseTree(ctx);
    
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.ArrayDeclarationContext ctx) {
        if (ctx.arrayName() is not { } arrayNameCtx) throw new Exception();
        if (ctx.literalArray() is not { } literalArrayCtx) throw new Exception();
        var name = arrayNameCtx.identifier() ?? throw new Exception();
        ArrayName = ctx.Start.InputStream.GetText(new Interval(name.Start.StartIndex, name.Stop.StopIndex));
        ArrayValue.ReadParseTree(literalArrayCtx);
        return this;
    }
}