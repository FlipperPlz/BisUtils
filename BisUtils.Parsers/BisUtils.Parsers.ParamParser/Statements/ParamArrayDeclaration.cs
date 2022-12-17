using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class ParamArrayDeclaration : ParamStatement, IComparable<ParamArrayDeclaration>, IParamParsable<Generated.ParamLang.ParamParser.ArrayDeclarationContext, ParamArrayDeclaration> {
    public string ArrayName { get; set; } = string.Empty;
    public ParamLiteralArray ArrayValue { get; set; } 
    
    public ParamArrayDeclaration(string arrayName, ParamLiteralArray array) {
        ArrayName = arrayName;
        ArrayValue = array;
    }
    
    public ParamArrayDeclaration() {}

    public ParamArrayDeclaration(Generated.ParamLang.ParamParser.ArrayDeclarationContext ctx) => FromParserContext(ctx);

    public ParamArrayDeclaration FromParserContext(Generated.ParamLang.ParamParser.ArrayDeclarationContext ctx) {
        if (ctx.arrayName() is not { } arrayNameCtx) throw new Exception();
        if (ctx.literalArray() is not { } literalArrayCtx) throw new Exception();
        var name = arrayNameCtx.identifier() ?? throw new Exception();
        ArrayName = ctx.Start.InputStream.GetText(new Interval(name.Start.StartIndex, name.Stop.StopIndex));
        ArrayValue = new ParamLiteralArray(literalArrayCtx);
        return this;
    }

    public void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));
        builder.Append(ArrayName).Append(" = ");
        ArrayValue.WriteString(builder, ParamSerializationOptions.Defaults);
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if (reader.ReadByte() != 2) throw new Exception("Expected external class.");
        ArrayName = reader.ReadAsciiZ();
        ArrayValue = reader.ReadBinarized<ParamLiteralArray>();
        
        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        writer.Write((byte) 2);
        writer.WriteAsciiZ(ArrayName);
        writer.WriteBinarized(ArrayValue);
    }
    
    public override int CompareTo(ParamStatement? other) {
        return other switch {
            ParamClassDeclaration => -4,
            ParamExternalClassStatement => -3,
            ParamDeleteStatement => -2,
            ParamAppensionStatement => -1,
            ParamArrayDeclaration arr => CompareTo(arr),
            ParamVariableDeclaration => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(ParamArrayDeclaration? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(ArrayName, other.ArrayName, StringComparison.Ordinal);
    }
}