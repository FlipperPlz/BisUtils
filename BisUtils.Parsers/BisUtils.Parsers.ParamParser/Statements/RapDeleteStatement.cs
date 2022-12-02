using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using BisUtils.Core;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Statements; 

public class RapDeleteStatement : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.DeleteStatementContext>, IComparable<RapDeleteStatement> {
    public string Target { get; set; } = null!;

    public RapDeleteStatement(string deleting) {
        Target = deleting;
    }

    public RapDeleteStatement() { }
    
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.DeleteStatementContext ctx) {
        if (ctx.identifier() is not { } identifier) throw new Exception("Nothing was given to delete.");
        Target = ctx.Start.InputStream.GetText(new Interval(identifier.Start.StartIndex, identifier.Stop.StopIndex));
        return this;
    }

    public static RapDeleteStatement FromContext(Generated.ParamLang.ParamParser.DeleteStatementContext ctx) =>
       (RapDeleteStatement) new RapDeleteStatement().ReadParseTree(ctx);

    public int CompareTo(IRapStatement? other) {
        return other switch {
            RapClassDeclaration => -2,
            RapExternalClassStatement => -1,
            RapDeleteStatement delete => CompareTo(delete),
            RapAppensionStatement => 1,
            RapArrayDeclaration => 2,
            RapVariableDeclaration => 3, 
            _ => throw new ArgumentOutOfRangeException(nameof(other), other, null)
        };
    }

    public int CompareTo(RapDeleteStatement? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return string.Compare(Target, other.Target, StringComparison.Ordinal);
    }

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.deleteStatement());
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();
        
        return this;
    }

    public void Write(StringBuilder builder, RapSerializationOptions serializationOptions) {
        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", serializationOptions.Indentation)));

        switch (serializationOptions.Language) {
            case ParamLanguage.CPP: {
                builder.Append("delete ").Append(Target).Append(';');
                return;
            }
            case ParamLanguage.XML: throw new NotSupportedException();
            default: throw new ArgumentOutOfRangeException(serializationOptions.Language.ToString());
        }
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
}