using System.Text;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Utils.Factories;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class ParamVariableDeclaration : ParamStatement, IComparable<ParamVariableDeclaration>, IParamParsable<Generated.ParamLang.ParamParser.TokenDeclarationContext, ParamVariableDeclaration> {
    public string VariableName { get; set; } = string.Empty;
    public IParamLiteral VariableValue { get; set; }

    public ParamVariableDeclaration(string variableName, IParamLiteral value) {
        VariableName = variableName;
        VariableValue = value;
    }

    public ParamVariableDeclaration() { }

    public ParamVariableDeclaration(Generated.ParamLang.ParamParser.TokenDeclarationContext ctx) =>
        FromParserContext(ctx);
    
    public ParamVariableDeclaration FromParserContext(Generated.ParamLang.ParamParser.TokenDeclarationContext ctx) {
        if (ctx.identifier() is not { } identifier) throw new Exception();
        if (ctx.value is not { } value) throw new Exception();
        VariableName = identifier.GetText();
        VariableValue = ParamLiteralFactory.Create(value);
        return this;
    }

    public void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));
        builder.Append(VariableName).Append(" = ");
        VariableValue.WriteString(builder, ParamSerializationOptions.Defaults);
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if (reader.ReadByte() != 1) throw new Exception("Expected token.");
        var valType = reader.ReadByte();
        VariableName = reader.ReadAsciiZ();
        VariableValue = valType switch {
            0 => reader.ReadBinarized<ParamLiteralString>(),
            1 => reader.ReadBinarized<ParamLiteralFloat>(),
            2 => reader.ReadBinarized<ParamLiteralInteger>(),
            _ => throw new Exception()
        };
        
        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
                writer.Write((byte) 1);
        switch (VariableValue) {
            case ParamLiteralString rapString: 
                writer.Write((byte) 0);
                writer.WriteAsciiZ(VariableName);
                writer.WriteBinarized(rapString);
                break;
            case ParamLiteralFloat rapFloat: 
                writer.Write((byte) 1);
                writer.WriteAsciiZ(VariableName);
                writer.WriteBinarized(rapFloat);
                break;
            case ParamLiteralInteger rapInteger: 
                writer.Write((byte) 2);
                writer.WriteAsciiZ(VariableName);
                writer.WriteBinarized(rapInteger);
                break;
        }
    }
    
    public override int CompareTo(ParamStatement? other) {
        return other switch {
            ParamClassDeclaration => -5,
            ParamExternalClassStatement => -4,
            ParamDeleteStatement => -3,
            ParamAppensionStatement => -2,
            ParamArrayDeclaration => -1,
            ParamVariableDeclaration var => CompareTo(var),
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(ParamVariableDeclaration? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(VariableName, other.VariableName, StringComparison.Ordinal);
    }
}