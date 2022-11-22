using System.Text;
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
    
    internal RapClassDeclaration() {}

    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.ClassDeclarationContext ctx) {
        if (ctx.classname is not { } classname) throw new Exception();
        Classname = classname.GetText();
        if (ctx.superclass is { } superclass) ParentClassname = superclass.GetText();
        if (ctx.statement() is { } statements) Statements = statements.Select(RapStatementFactory.Create).ToList();
        return this;
    }

    public string ToString(int indentation = 0) {
        var builder = new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
            .Append("class ").Append(Classname);
        if (ParentClassname is not null) builder.Append(" : ").Append(ParentClassname);
        builder.Append(" {\n");
        
        var statements = Statements;
        Statements.Sort((x, y) => x.CompareTo(y));
        foreach (var s in statements) builder.Append(s.ToString(indentation + 1)).Append('\n');
        
        return builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", indentation))).Append("};").ToString();
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
}