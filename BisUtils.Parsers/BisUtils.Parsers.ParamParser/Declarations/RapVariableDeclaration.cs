using System.Text;
using Antlr4.Runtime;
using BisUtils.Core;
using BisUtils.Core.Serialization;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Statements;

namespace BisUtils.Parsers.ParamParser.Declarations; 

public class RapVariableDeclaration : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.TokenDeclarationContext>, IComparable<RapVariableDeclaration> {
    public string VariableName { get; set; } = string.Empty;
    public IRapLiteral VariableValue { get; set; }

    public RapVariableDeclaration(string variableName, IRapLiteral value) {
        VariableName = variableName;
        VariableValue = value;
    }

    public RapVariableDeclaration() { }
    
    public static RapVariableDeclaration FromContext(Generated.ParamLang.ParamParser.TokenDeclarationContext ctx) =>
        (RapVariableDeclaration) new RapVariableDeclaration().ReadParseTree(ctx);

    
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.TokenDeclarationContext ctx) {
        if (ctx.identifier() is not { } identifier) throw new Exception();
        if (ctx.value is not { } value) throw new Exception();
        VariableName = ctx.identifier().GetText();
        VariableValue = RapLiteralFactory.Create(value);
        return this;
    }

    public int CompareTo(IRapStatement? other) {
        return other switch {
            RapClassDeclaration => -5,
            RapExternalClassStatement => -4,
            RapDeleteStatement => -3,
            RapAppensionStatement => -2,
            RapArrayDeclaration => -1,
            RapVariableDeclaration var => CompareTo(var),
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(RapVariableDeclaration? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(VariableName, other.VariableName, StringComparison.Ordinal);
    }

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.tokenDeclaration());
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();
        
        return this;
    }

    public void Write(StringBuilder builder, RapSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));

        switch (serializationOptions.Language) {
            case ParamLanguage.CPP: {
                builder.Append(VariableName).Append(" = ");
                VariableValue.Write(builder, RapSerializationOptions.DefaultOptions);
                return;
            }
            case ParamLanguage.XML: throw new NotSupportedException();
            default: throw new ArgumentOutOfRangeException(serializationOptions.Language.ToString());
        }
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if (reader.ReadByte() != 1) throw new Exception("Expected token.");
        var valType = reader.ReadByte();
        VariableName = reader.ReadAsciiZ();
        VariableValue = valType switch {
            0 => reader.ReadBinarized<RapString>(),
            1 => reader.ReadBinarized<RapFloat>(),
            2 => reader.ReadBinarized<RapInteger>(),
            _ => throw new Exception()
        };
        
        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        writer.Write((byte) 1);
        switch (VariableValue) {
            case RapString rapString: 
                writer.Write((byte) 0);
                writer.WriteAsciiZ(VariableName);
                writer.WriteBinarized(rapString);
                break;
            case RapFloat rapFloat: 
                writer.Write((byte) 1);
                writer.WriteAsciiZ(VariableName);
                writer.WriteBinarized(rapFloat);
                break;
            case RapInteger rapInteger: 
                writer.Write((byte) 2);
                writer.WriteAsciiZ(VariableName);
                writer.WriteBinarized(rapInteger);
                break;
        }
    }
}