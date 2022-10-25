using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals; 

public class RapString : IRapDeserializable<Generated.ParamLang.ParamParser.LiteralStringContext>, IRapArrayEntry {
    public string Value { get; private set; }
    public static implicit operator RapString(string s) => new(s);
    public static implicit operator string(RapString s) => s.Value;
    public RapString(string s) => Value = s;
    public string ToString(int indentation = char.MinValue) =>
        new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation))).Append('"').Append(Value).Append('"').ToString();
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.LiteralStringContext ctx) {
        Value = ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)).TrimStart('"').TrimEnd('"');
        return this;
    }

    private RapString() {}

    public static RapString FromContext(Generated.ParamLang.ParamParser.LiteralStringContext ctx) =>
        (RapString) new RapString().ReadParseTree(ctx);
}