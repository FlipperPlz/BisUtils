using System.Text;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Utils.Factories;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class ParamClassDeclaration : ParamStatement, IComparable<ParamClassDeclaration>, IParamParsable<Generated.ParamLang.ParamParser.ClassDeclarationContext, ParamClassDeclaration> {
    public ParamClassDeclaration(Generated.ParamLang.ParamParser.ClassDeclarationContext ctx) => FromParserContext(ctx);

    public ParamClassDeclaration() { }

    public string Classname { get; set; } = string.Empty;
    public string? ParentClassname { get; set; } = null;
    public List<ParamStatement> Statements { get; set; }

    public uint BinaryOffset { get; set; } = 0;
    public long BinaryOffsetPosition { get; set; } = 0;

    public ParamClassDeclaration FromParserContext(Generated.ParamLang.ParamParser.ClassDeclarationContext ctx) {
        if (ctx.classname is not { } classname) throw new Exception();
        Classname = classname.GetText();
        if (ctx.superclass is { } superclass) ParentClassname = superclass.GetText();
        if (ctx.statement() is { } statements) Statements = statements.Select(ParamStatementFactory.Create).ToList();
        return this;
    }

    public void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation))).Append("class ").Append(Classname);
        if (ParentClassname is not null) builder.Append(" : ").Append(ParentClassname);
        builder.Append(" {\n");
        
        var statements = Statements;
        if(serializationOptions.OrganizeEntries) Statements.Sort((x, y) => x.CompareTo(y));

        foreach (var s in statements) {
            ParamStatementFactory.CreateSerializable(s).WriteString(builder, new ParamSerializationOptions() { Indentation = serializationOptions.Indentation + 1});
            builder.Append('\n');
        }

        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)))
            .Append("};");
        return;
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if ( reader.ReadByte() != 0) throw new Exception($"Expected class.");
        Statements = new();
        Classname = reader.ReadAsciiZ();
        BinaryOffsetPosition = reader.BaseStream.Position;
        BinaryOffset = reader.ReadUInt32();

        return this;
    }

    public void WriteBinary(BinaryWriter writer) {
        writer.Write((byte) 0);
        writer.WriteAsciiZ(Classname);
        BinaryOffsetPosition = writer.BaseStream.Position;
        writer.Write((uint) BinaryOffset);
    }
    
    public override int CompareTo(ParamStatement? other) {
        return other switch {
            ParamClassDeclaration clazz => CompareTo(clazz),
            ParamExternalClassStatement => 1,
            ParamDeleteStatement => 2,
            ParamAppensionStatement => 3,
            ParamArrayDeclaration => 4,
            ParamVariableDeclaration => 5,
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(ParamClassDeclaration? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Classname, other.Classname, StringComparison.Ordinal);
    }
}