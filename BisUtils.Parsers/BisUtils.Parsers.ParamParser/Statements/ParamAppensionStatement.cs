using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class ParamAppensionStatement : ParamStatement, IComparable<ParamAppensionStatement>, IParamParsable<Generated.ParamLang.ParamParser.ArrayAppensionContext, ParamAppensionStatement> {
    public string Target { get; set; } = string.Empty;
    public ParamLiteralArray Array { get; set; } = ParamLiteralArray.EmptyArray;

    public ParamAppensionStatement(string arrayName, ParamLiteralArray array) {
        Target = arrayName;
        Array = array;
    }
    
    public ParamAppensionStatement() {}

    public ParamAppensionStatement(Generated.ParamLang.ParamParser.ArrayAppensionContext ctx) => FromParserContext(ctx);

    public ParamAppensionStatement FromParserContext(Generated.ParamLang.ParamParser.ArrayAppensionContext ctx) {
        if (ctx.arrayName() is not { } arrayNameCtx) throw new Exception();
        if (ctx.literalArray() is not { } literalArrayCtx) throw new Exception();
        Target = ctx.Start.InputStream.GetText(new Interval(arrayNameCtx.identifier().Start.StartIndex, arrayNameCtx.identifier().Stop.StopIndex));
        Array = new ParamLiteralArray(literalArrayCtx);
        return this;
    }

    public void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)))
            .Append(Target).Append("[] += ").Append(Array).Append(';');
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if (reader.ReadByte() != 5) throw new Exception("Expected array appension.");
        if (reader.ReadInt32() != 1) throw new Exception("Expected array appension. (1)");
        Target = reader.ReadAsciiZ();
        Array = reader.ReadBinarized<ParamLiteralArray>();
        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        writer.Write((byte) 5);
        writer.Write((int) 1);
        writer.WriteAsciiZ(Target);
        writer.WriteBinarized(Array);
    }
    
    public override int CompareTo(ParamStatement? other) {
        return other switch {
            ParamClassDeclaration => -3,
            ParamExternalClassStatement => -2,
            ParamDeleteStatement => -1,
            ParamAppensionStatement append => CompareTo(append),
            ParamArrayDeclaration => 1,
            ParamVariableDeclaration => 2, 
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(ParamAppensionStatement? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Target, other.Target, StringComparison.Ordinal);
    }
}