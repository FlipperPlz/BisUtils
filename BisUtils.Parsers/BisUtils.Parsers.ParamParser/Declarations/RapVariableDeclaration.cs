using System.Text;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;

namespace BisUtils.Parsers.ParamParser.Declarations; 

public class RapVariableDeclaration : IRapStatement, IRapDeserializable<Generated.ParamLang.ParamParser.TokenDeclarationContext> {
    public string VariableName { get; set; } = string.Empty;
    public IRapLiteral VariableValue { get; set; }

    public RapVariableDeclaration(string variableName, IRapLiteral value) {
        VariableName = variableName;
        VariableValue = value;
    }

    private RapVariableDeclaration() { }

    public string ToString(int indentation = char.MinValue) => new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
        .Append(VariableName).Append(" = ").Append(VariableValue.ToString()).Append(';').ToString();
    
    public static RapVariableDeclaration FromContext(Generated.ParamLang.ParamParser.TokenDeclarationContext ctx) =>
        (RapVariableDeclaration) new RapVariableDeclaration().ReadParseTree(ctx);

    
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.TokenDeclarationContext ctx) {
        if (ctx.identifier() is not { } identifier) throw new Exception();
        if (ctx.value is not { } value) throw new Exception();
        VariableName = ctx.identifier().GetText();
        VariableValue = RapLiteralFactory.Create(value);
        return this;
    }
}