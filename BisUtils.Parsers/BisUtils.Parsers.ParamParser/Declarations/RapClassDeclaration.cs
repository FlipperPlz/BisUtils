using System.Text;
using Antlr4.Runtime;
using BisUtils.Core;
using BisUtils.Core.Serialization;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Statements;

namespace BisUtils.Parsers.ParamParser.Declarations; 

public class RapClassDeclaration : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.ClassDeclarationContext>, IComparable<IRapStatement>, IComparable<RapClassDeclaration> {
    public string Classname { get; set; } = string.Empty;
    public string? ParentClassname { get; set; } = null;
    public List<IRapStatement> Statements { get; set; }

    public uint BinaryOffset { get; set; } = 0;
    public long BinaryOffsetPosition { get; set; } = 0;
    
    public RapClassDeclaration(string classname, string? parentClassname = null,
        IEnumerable<IRapStatement>? statements = null) {
        statements ??= new List<IRapStatement>();
        Classname = classname;
        ParentClassname = parentClassname;
        Statements = statements.ToList();
    }

    public RapClassDeclaration() {}

    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.ClassDeclarationContext ctx) {
        if (ctx.classname is not { } classname) throw new Exception();
        Classname = classname.GetText();
        if (ctx.superclass is { } superclass) ParentClassname = superclass.GetText();
        if (ctx.statement() is { } statements) Statements = statements.Select(RapStatementFactory.Create).ToList();
        return this;
    }

    public static RapClassDeclaration FromContext(Generated.ParamLang.ParamParser.ClassDeclarationContext ctx) =>
        (RapClassDeclaration) new RapClassDeclaration().ReadParseTree(ctx);

    public int CompareTo(RapClassDeclaration? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Classname, other.Classname, StringComparison.Ordinal);
    }

    public int CompareTo(IRapStatement? other) {
        return other switch {
            RapClassDeclaration clazz => CompareTo(clazz),
            RapExternalClassStatement => 1,
            RapDeleteStatement => 2,
            RapAppensionStatement => 3,
            RapArrayDeclaration => 4,
            RapVariableDeclaration => 5,
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.classDeclaration());
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();
        
        return this;
    }

    public void Write(StringBuilder builder, RapSerializationOptions serializationOptions) {
        switch (serializationOptions.Language) {
            case ParamLanguage.CPP: {
                builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation))).Append("class ").Append(Classname);
                if (ParentClassname is not null) builder.Append(" : ").Append(ParentClassname);
                builder.Append(" {\n");
        
                var statements = Statements;
                if(serializationOptions.OrganizeEntries) Statements.Sort((x, y) => x.CompareTo(y));

                foreach (var s in statements) {
                    s.Write(builder, new RapSerializationOptions() { Indentation = serializationOptions.Indentation + 1});
                    builder.Append('\n');
                }

                builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)))
                    .Append("};");
                return;
            }
            case ParamLanguage.XML: throw new NotSupportedException();
            default:
                throw new ArgumentOutOfRangeException(serializationOptions.Language.ToString());
        }
        
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        if ( reader.ReadByte() != 0) throw new Exception($"Expected class.");
        Statements = new List<IRapStatement>();
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
}