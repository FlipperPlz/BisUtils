using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class ParamDeleteStatement : ParamStatement, IComparable<ParamDeleteStatement>, IParamParsable<Generated.ParamLang.ParamParser.DeleteStatementContext, ParamDeleteStatement> {
    public string Target { get; set; } = null!;

    public ParamDeleteStatement(string deleting) => Target = deleting;
    public ParamDeleteStatement() { }

    public ParamDeleteStatement(Generated.ParamLang.ParamParser.DeleteStatementContext ctx) => FromParserContext(ctx);

    public ParamDeleteStatement FromParserContext(Generated.ParamLang.ParamParser.DeleteStatementContext ctx) {
        if (ctx.identifier() is not { } identifier) throw new Exception("Nothing was given to delete.");
        Target = ctx.Start.InputStream.GetText(new Interval(identifier.Start.StartIndex, identifier.Stop.StopIndex));
        return this;
    }

    public void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));
        builder.Append("delete ").Append(Target).Append(';');
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if (reader.ReadByte() != 4) throw new Exception("Expected delete statement.");
        Target = reader.ReadAsciiZ();
        
        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        writer.Write((byte) 4);
        writer.WriteAsciiZ(Target);
    }

    public override int CompareTo(ParamStatement? other) {
        return other switch {
            ParamClassDeclaration => -2,
            ParamExternalClassStatement => -1,
            ParamDeleteStatement delete => CompareTo(delete),
            ParamAppensionStatement => 1,
            ParamArrayDeclaration => 2,
            ParamVariableDeclaration => 3, 
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(ParamDeleteStatement? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Target, other.Target, StringComparison.Ordinal);
    }
}