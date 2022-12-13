using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using BisUtils.Core;
using BisUtils.Core.Serialization;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals;

public class RapFloat : IRapDeserializable<Generated.ParamLang.ParamParser.LiteralFloatContext>, IRapArrayEntry {
    public float Value { get; set; } = 0.0f;
    public static implicit operator RapFloat(float s) => new(s);
    public static implicit operator float(RapFloat s) => s.Value;
    public RapFloat(float f) => Value = f;
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.LiteralFloatContext ctx) {
        Value = float.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
        return this;
    }

    public RapFloat() {}

    public static RapFloat FromContext(Generated.ParamLang.ParamParser.LiteralFloatContext ctx) =>
        (RapFloat) new RapFloat().ReadParseTree(ctx);

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.literalFloat());
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();
        
        return this;
    }

    public void Write(StringBuilder builder, RapSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));

        switch (serializationOptions.Language) {
            case ParamLanguage.CPP: {
                builder.Append(Value);
                return;
            }
            case ParamLanguage.XML: {
                builder.Append("<item>").Append(Value).Append("</item>");
                return;
            }
            default: throw new ArgumentOutOfRangeException(serializationOptions.Language.ToString());
        }
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        Value = reader.ReadSingle();
        
        return this;
    }

    public void WriteBinary(BinaryWriter writer) => writer.Write(Value);
}