namespace BisUtils.PreProcessor.RV.Models.Directives;

using System.Text;
using FResults;
using Lexer;
using Stubs;

public interface IRVIfDefinedDirective : IRVDirective
{
    string MacroName { get; }
    string IfBody { get; }
    string? ElseBody { get; }
}


public class RVIfDefinedDirective : RVDirective, IRVIfDefinedDirective
{
    public string MacroName { get; }
    public string IfBody { get; }
    public string? ElseBody { get; }

    public RVIfDefinedDirective(IRVPreProcessor processor, string macroName, string ifBody, string? elseBody = null) : base(processor, "ifdef")
    {
        MacroName = macroName;
        IfBody = ifBody;
        ElseBody = elseBody;
    }

    protected RVIfDefinedDirective(IRVPreProcessor processor,string directiveName, string macroName, string ifBody, string? elseBody = null) : base(processor, directiveName)
    {
        MacroName = macroName;
        IfBody = ifBody;
        ElseBody = elseBody;
    }

    public override Result ToText(out string str)
    {
        var builder = new StringBuilder(DirectiveKeyword);
        builder.Append(MacroName).Append('\n').Append(IfBody).Append('\n');
        if (string.IsNullOrWhiteSpace(ElseBody))
        {
            builder.Append("#else").Append('\n').Append(ElseBody).Append('\n');
        }

        str = builder.Append("#endif").ToString();
        return Result.ImmutableOk();
    }

    public override Result Process(RVLexer lexer, int startPosition) => throw new NotImplementedException();
}
