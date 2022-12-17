using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class ParamExternalClassStatement : ParamStatement, IComparable<ParamExternalClassStatement>, IParamParsable<Generated.ParamLang.ParamParser.ExternalClassDeclarationContext, ParamExternalClassStatement> {
    public string Classname { get; set; } = null!;

    public ParamExternalClassStatement(string classname) => Classname = classname;
    public ParamExternalClassStatement() { }

    public ParamExternalClassStatement(Generated.ParamLang.ParamParser.ExternalClassDeclarationContext ctx) =>
        FromParserContext(ctx);
    public ParamExternalClassStatement FromParserContext(Generated.ParamLang.ParamParser.ExternalClassDeclarationContext ctx) {
        if (ctx.classname is not { } classname) throw new Exception();
        Classname = ctx.Start.InputStream.GetText(new Interval(classname.Start.StartIndex, classname.Stop.StopIndex));
        return this;
    }

    public void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));
        builder.Append("class ").Append(Classname).Append(';');
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
    
    public override int CompareTo(ParamStatement? other) {
        return other switch {
            ParamClassDeclaration => -1,
            ParamExternalClassStatement external => CompareTo(external),
            ParamDeleteStatement => 1,
            ParamAppensionStatement => 2,
            ParamArrayDeclaration => 3,
            ParamVariableDeclaration => 4, 
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(ParamExternalClassStatement? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Classname, other.Classname, StringComparison.Ordinal);
    }
}