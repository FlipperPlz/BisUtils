using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using BisUtils.Core;
using BisUtils.Core.Serialization;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Statements;

namespace BisUtils.Parsers.ParamParser.Declarations; 

public class RapArrayDeclaration :  IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.ArrayDeclarationContext>, IComparable<RapArrayDeclaration> {
    public string ArrayName { get; set; } = string.Empty;
    public RapArray ArrayValue { get; set; } = RapArray.EmptyArray;
    
    public RapArrayDeclaration(string arrayName, RapArray array) {
        ArrayName = arrayName;
        ArrayValue = array;
    }
    
    public RapArrayDeclaration() {}
    
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

    public int CompareTo(IRapStatement? other) {
        return other switch {
            RapClassDeclaration => -4,
            RapExternalClassStatement => -3,
            RapDeleteStatement => -2,
            RapAppensionStatement => -1,
            RapArrayDeclaration arr => CompareTo(arr),
            RapVariableDeclaration => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(RapArrayDeclaration? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(ArrayName, other.ArrayName, StringComparison.Ordinal);
    }

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.arrayDeclaration());
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();
        
        return this;
    }

    public void Write(StringBuilder builder, RapSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));

        switch (serializationOptions.Language) {
            case ParamLanguage.CPP: {
                builder.Append(ArrayName).Append(" = ");
                ArrayValue.Write(builder, RapSerializationOptions.DefaultOptions);
                return;
            }
            case ParamLanguage.XML: throw new NotSupportedException();
            default: throw new ArgumentOutOfRangeException(serializationOptions.Language.ToString());
        }
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if (reader.ReadByte() != 2) throw new Exception("Expected external class.");
        ArrayName = reader.ReadAsciiZ();
        ArrayValue = reader.ReadBinarized<RapArray>();
        
        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        writer.Write((byte) 2);
        writer.WriteAsciiZ(ArrayName);
        writer.WriteBinarized(ArrayValue);
    }
}