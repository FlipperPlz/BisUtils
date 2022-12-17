using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals; 

public class ParamLiteralString : ParamLiteral<string, Generated.ParamLang.ParamParser.LiteralStringContext, ParamLiteralString> {
    public ParamLiteralString() {}
    public ParamLiteralString(string value) : base(value) { }
    public ParamLiteralString(Generated.ParamLang.ParamParser.LiteralStringContext value) : base(value) { }
    public ParamLiteralString(BinaryReader value) : base(value) { }
    
    public override ParamLiteralString FromParserContext(Generated.ParamLang.ParamParser.LiteralStringContext ctx) {
        Value = ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)).TrimStart('"').TrimEnd('"');
        return this;
    }
    

    public override void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));
        builder.Append('"').Append(Value).Append('"');
    }
    
    public override IBisBinarizable ReadBinary(BinaryReader reader) {
        Value = reader.ReadAsciiZ();
        
        return this;
    }

    public override void WriteBinary(BinaryWriter writer) => writer.WriteAsciiZ(Value);
}