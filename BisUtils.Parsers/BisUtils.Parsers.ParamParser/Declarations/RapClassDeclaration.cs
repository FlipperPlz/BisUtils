using System.Text;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Declarations; 

public class RapClassDeclaration : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.ClassDeclarationContext> {
    public string Classname { get; set; } = string.Empty;
    public string? ParentClassname { get; set; } = null;
    public IEnumerable<IRapStatement> Statements { get; set; }

    public uint BinaryOffset { get; set; } = 0;
    public long BinaryOffsetPosition { get; set; } = 0;


    public RapClassDeclaration(string classname, string? parentClassname = null,
        IEnumerable<IRapStatement>? statements = null) {
        statements ??= new List<IRapStatement>();
        Classname = classname;
        ParentClassname = parentClassname;
        Statements = statements;
    }
    
    private RapClassDeclaration() {}

    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.ClassDeclarationContext ctx) {
        if (ctx.classname is not { } classname) throw new Exception();
        Classname = classname.GetText();
        if (ctx.superclass is { } superclass) ParentClassname = superclass.GetText();
        if (ctx.statement() is { } statements) Statements = statements.Select(RapStatementFactory.Create);
        return this;
    }

    public string ToString(int indentation = 0) {
        var builder = new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
            .Append("class ").Append(Classname);
        if (ParentClassname is not null) builder.Append(" : ").Append(ParentClassname);
        builder.Append(" {\n");
        foreach (var s in Statements) builder.Append(s.ToString(indentation + 1)).Append('\n');
        return builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", indentation))).Append("};").ToString();
    }
    
    public static RapClassDeclaration FromContext(Generated.ParamLang.ParamParser.ClassDeclarationContext ctx) =>
        (RapClassDeclaration) new RapClassDeclaration().ReadParseTree(ctx);


}