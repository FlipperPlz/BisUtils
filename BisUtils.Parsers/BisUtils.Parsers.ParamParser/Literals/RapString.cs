using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using BisUtils.Core;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals; 

public class RapString : IRapDeserializable<Generated.ParamLang.ParamParser.LiteralStringContext>, IRapArrayEntry {
    public string Value { get; set; }
    public static implicit operator RapString(string s) => new(s);
    public static implicit operator string(RapString s) => s.Value;
    public RapString(string s) => Value = s;
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.LiteralStringContext ctx) {
        Value = ctx.Start.InputStream.GetText(new Interval(ctx.Start.StartIndex, ctx.Stop.StopIndex)).TrimStart('"').TrimEnd('"');
        return this;
    }

    public RapString() {}

    public static RapString FromContext(Generated.ParamLang.ParamParser.LiteralStringContext ctx) =>
        (RapString) new RapString().ReadParseTree(ctx);

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.literalString());
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();
        
        return this;
    }

    public void Write(StringBuilder builder, RapSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));

        switch (serializationOptions.Language) {
            case ParamLanguage.CPP: {
                builder.Append('"').Append(Value).Append('"');
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
        Value = reader.ReadAsciiZ();
        
        return this;
    }

    public void WriteBinary(BinaryWriter writer) => writer.WriteAsciiZ(Value);
}