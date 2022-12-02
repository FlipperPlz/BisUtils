using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using BisUtils.Core;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class RapAppensionStatement : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.ArrayAppensionContext>, IComparable<RapAppensionStatement>{
    public string Target { get; set; } = string.Empty;
    public RapArray Array { get; set; } = RapArray.EmptyArray;

    public RapAppensionStatement(string arrayName, RapArray array) {
        Target = arrayName;
        Array = array;
    }
    
    public RapAppensionStatement() {}
    

    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.ArrayAppensionContext ctx) {
        if (ctx.arrayName() is not { } arrayNameCtx) throw new Exception();
        if (ctx.literalArray() is not { } literalArrayCtx) throw new Exception();
        Target = ctx.Start.InputStream.GetText(new Interval(arrayNameCtx.identifier().Start.StartIndex, arrayNameCtx.identifier().Stop.StopIndex));
        Array.ReadParseTree(literalArrayCtx);
        return this;
    }

    public static RapAppensionStatement FromContext(Generated.ParamLang.ParamParser.ArrayAppensionContext ctx) =>
        (RapAppensionStatement) new RapAppensionStatement().ReadParseTree(ctx);

    public int CompareTo(IRapStatement? other) {
        return other switch {
            RapClassDeclaration => -3,
            RapExternalClassStatement => -2,
            RapDeleteStatement => -1,
            RapAppensionStatement append => CompareTo(append),
            RapArrayDeclaration => 1,
            RapVariableDeclaration => 2, 
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(RapAppensionStatement? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Target, other.Target, StringComparison.Ordinal);
    }

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.arrayAppension());
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();
        
        return this;
    }

    public void Write(StringBuilder builder, RapSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));

        switch (serializationOptions.Language) {
            case ParamLanguage.CPP: {
                builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)))
                    .Append(Target).Append("[] += ").Append(Array.ToString()).Append(';');
                return;
            }
            case ParamLanguage.XML: throw new NotSupportedException();
            default: throw new ArgumentOutOfRangeException(serializationOptions.Language.ToString());
        }
    }
        


    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if (reader.ReadByte() != 5) throw new Exception("Expected array appension.");
        if (reader.ReadInt32() != 1) throw new Exception("Expected array appension. (1)");
        Target = reader.ReadAsciiZ();
        Array = reader.ReadBinarized<RapArray>();
        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        writer.Write((byte) 5);
        writer.Write((int) 1);
        writer.WriteAsciiZ(Target);
        writer.WriteBinarized<RapArray>(Array);
    }
}