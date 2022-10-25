using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals;

public class RapFloat : IRapDeserializable<Generated.ParamLang.ParamParser.LiteralFloatContext>, IRapArrayEntry {
    public float Value { get; set; } = 0.0f;
    public static implicit operator RapFloat(float s) => new(s);
    public static implicit operator float(RapFloat s) => s.Value;
    public RapFloat(float f) => Value = f;
    public string ToString(int indentation = char.MinValue) =>
        new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
            .Append(Value.ToString("G")).ToString();
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.LiteralFloatContext ctx) {
        Value = float.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
        return this;
    }
    
    private RapFloat() {}

    public static RapFloat FromContext(Generated.ParamLang.ParamParser.LiteralFloatContext ctx) =>
        (RapFloat) new RapFloat().ReadParseTree(ctx);
}