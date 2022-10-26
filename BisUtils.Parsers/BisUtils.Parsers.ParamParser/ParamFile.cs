using Antlr4.Runtime;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser; 

public class ParamFile {
    public List<IRapStatement> Statements { get; set; } = new();
    
    public ParamFile ReadParseTree(Generated.ParamLang.ParamParser.ComputationalStartContext ctx) {
        if (ctx.statement() is { } statements) Statements.AddRange(statements.Select(RapStatementFactory.Create));
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