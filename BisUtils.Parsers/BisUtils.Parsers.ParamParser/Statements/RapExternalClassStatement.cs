using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using BisUtils.Core;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class RapExternalClassStatement : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.ExternalClassDeclarationContext>, IComparable<RapExternalClassStatement> {
    public string Classname { get; set; }

    public RapExternalClassStatement(string classname) => Classname = classname;
    public RapExternalClassStatement() { }

    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.ExternalClassDeclarationContext ctx) {
        if (ctx.classname is not { } classname) throw new Exception();
        Classname = ctx.Start.InputStream.GetText(new Interval(classname.Start.StartIndex, classname.Stop.StopIndex));
        return this;
    }

    public static RapExternalClassStatement FromContext(Generated.ParamLang.ParamParser.ExternalClassDeclarationContext ctx) =>
        (RapExternalClassStatement) new RapExternalClassStatement().ReadParseTree(ctx);

    public int CompareTo(IRapStatement? other) {
        return other switch {
            RapClassDeclaration => -1,
            RapExternalClassStatement external => CompareTo(external),
            RapDeleteStatement => 1,
            RapAppensionStatement => 2,
            RapArrayDeclaration => 3,
            RapVariableDeclaration => 4, 
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(RapExternalClassStatement? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Classname, other.Classname, StringComparison.Ordinal);
    }

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.externalClassDeclaration());
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();
        
        return this;
    }

    public void Write(StringBuilder builder, RapSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));

        switch (serializationOptions.Language) {
            case ParamLanguage.CPP: {
                builder.Append("class ").Append(Classname).Append(';');
                return;
            }
            case ParamLanguage.XML: throw new NotSupportedException();
            default: throw new ArgumentOutOfRangeException(serializationOptions.Language.ToString());
        }
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if (reader.ReadByte() != 3) throw new Exception("Expected external class.");
        Classname = reader.ReadAsciiZ();

        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        writer.Write((byte) 3);
        writer.WriteAsciiZ(Classname);
    }
}