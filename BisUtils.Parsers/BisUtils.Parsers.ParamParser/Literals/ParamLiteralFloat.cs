using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals; 

public class ParamLiteralFloat : ParamLiteral<float, Generated.ParamLang.ParamParser.LiteralFloatContext, ParamLiteralFloat> {
    public ParamLiteralFloat() { }
    public ParamLiteralFloat(float value = 0) : base(value) { }
    public ParamLiteralFloat(Generated.ParamLang.ParamParser.LiteralFloatContext value) : base(value) { }
    public ParamLiteralFloat(BinaryReader value) : base(value) { }

    public override ParamLiteralFloat FromParserContext(Generated.ParamLang.ParamParser.LiteralFloatContext ctx) {
        Value = float.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
        return this;
    }

    public override void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));
        builder.Append(Value);
    }

    public override IBisBinarizable ReadBinary(BinaryReader reader) {
        Value = reader.ReadSingle();
        
        return this;
    }

    public override void WriteBinary(BinaryWriter writer)  => writer.Write(Value);
}