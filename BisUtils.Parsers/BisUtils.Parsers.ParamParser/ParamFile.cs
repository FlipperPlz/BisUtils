using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;

namespace BisUtils.Parsers.ParamParser; 

public class ParamFile {
    public List<IRapStatement> Statements { get; set; } = new();
    public Dictionary<string, int?>? EnumValues { get; set; } = null;
    public ParamFile ReadParseTree(Generated.ParamLang.ParamParser.ComputationalStartContext ctx) {
        if (ctx.statement() is { } statements) Statements.AddRange(statements.Select(RapStatementFactory.Create));
        if (ctx.enumDeclaration() is { } enums) {
            EnumValues = new Dictionary<string, int?>();
            foreach (var e in enums) {
                foreach (var enumValueContext in e.enumValue()) {

                    EnumValues.Add(ctx.Start.InputStream.GetText(new Interval(
                            enumValueContext.identifier().Start.StartIndex,
                            enumValueContext.identifier().Stop.StopIndex)),
                        enumValueContext.literalInteger() is { } integerContext
                            ? RapInteger.FromContext(integerContext).Value
                            : null);

                }
            }
        }
        return this;
    }
    
    public static ParamFile ParseParamFile(Stream stream) {
        var memStream = new MemoryStream();
        stream.Seek(0, SeekOrigin.Begin);
        stream.CopyTo(memStream);
        memStream.Seek(0, SeekOrigin.Begin);

        var lexer = new ParamLexer(CharStreams.fromStream(memStream));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);
           
        var computationalStart = parser.computationalStart();
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();

        return (ParamFile) new ParamFile().ReadParseTree(computationalStart);
    }
    
}