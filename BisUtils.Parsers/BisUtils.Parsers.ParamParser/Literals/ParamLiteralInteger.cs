using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals; 

public class ParamLiteralInteger : ParamLiteral<int, Generated.ParamLang.ParamParser.LiteralIntegerContext, ParamLiteralInteger> {
    public ParamLiteralInteger() { }
    public ParamLiteralInteger(int value) : base(value) { }
    public ParamLiteralInteger(Generated.ParamLang.ParamParser.LiteralIntegerContext value) : base(value) { }
    public ParamLiteralInteger(BinaryReader value) : base(value) { }

    public override ParamLiteralInteger FromParserContext(Generated.ParamLang.ParamParser.LiteralIntegerContext ctx) {
        Value = int.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
        return this;
    }

    public override void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));
        builder.Append(Value);
    }

    public override IBisBinarizable ReadBinary(BinaryReader reader) {
        Value = reader.ReadInt32();

        return this;
    }

    public override void WriteBinary(BinaryWriter writer) => writer.Write(Value);
}