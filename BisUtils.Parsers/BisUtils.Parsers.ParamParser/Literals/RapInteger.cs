using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals; 

public class RapInteger : IRapDeserializable<Generated.ParamLang.ParamParser.LiteralIntegerContext>, IRapLiteral, IRapArrayEntry {
    public int Value { get; set; }
    public static implicit operator RapInteger(int s) => new(s);
    public static implicit operator int (RapInteger s) => s.Value;

    public RapInteger(int val = 0) => Value = val;

    public string ToString(int indentation = char.MinValue) =>
        new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
            .Append(Value).ToString();

    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.LiteralIntegerContext ctx) {
        Value = int.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
        return this;
    }
    
    private RapInteger() {}

    public static RapInteger FromContext(Generated.ParamLang.ParamParser.LiteralIntegerContext ctx) =>
        (RapInteger) new RapInteger().ReadParseTree(ctx);
}