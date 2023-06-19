namespace BisUtils.PreProcessor.RV.Models.Directives;

using FResults;
using Lexer;
using Stubs;

public interface IRVUndefineDirective : IRVDirective
{
    string MacroName { get; }
}

public class RVUndefineDirective: RVDirective, IRVUndefineDirective
{
    public string MacroName { get; set; }

    public RVUndefineDirective(IRVPreProcessor processor, string macroName) : base(processor, "undef") =>
        MacroName = macroName;

    public override Result ToText(out string str)
    {
        str = MacroName;
        return Result.ImmutableOk();
    }

    public override Result Process(RVLexer lexer, int startPosition) => throw new NotImplementedException();
}
