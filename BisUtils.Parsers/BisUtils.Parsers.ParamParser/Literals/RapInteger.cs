using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using BisUtils.Core;
using BisUtils.Core.Serialization;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals; 

public class RapInteger : IRapDeserializable<Generated.ParamLang.ParamParser.LiteralIntegerContext>, IRapLiteral, IRapArrayEntry {
    public int Value { get; set; }
    public static implicit operator RapInteger(int s) => new(s);
    public static implicit operator int (RapInteger s) => s.Value;

    public RapInteger(int val = 0) => Value = val;

    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.LiteralIntegerContext ctx) {
        Value = int.Parse(ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)));
        return this;
    }

    public RapInteger() {}

    public static RapInteger FromContext(Generated.ParamLang.ParamParser.LiteralIntegerContext ctx) =>
        (RapInteger) new RapInteger().ReadParseTree(ctx);

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.literalInteger());
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
        Value = reader.ReadInt32();

        return this;
    }

    public void WriteBinary(BinaryWriter writer) => writer.Write(Value);
}