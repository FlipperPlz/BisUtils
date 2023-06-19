namespace BisUtils.PreProcessor.RV.Models.Elements;

using BisUtils.PreProcessor.RV.Models.Stubs;
using FResults;

public interface IRVMacro : IRVElement
{
    string MacroName { get; }
    List<string> MacroArguments { get; }
}

public class RVMacro : RVElement, IRVMacro
{
    public string MacroName { get; }
    public List<string> MacroArguments { get; }

    public RVMacro(IRVPreProcessor preProcessor, string macroName, List<string>? macroArguments = null) : base(preProcessor)
    {
        MacroName = macroName;
        MacroArguments = macroArguments ?? new List<string>();
    }

    public override Result ToText(out string str)
    {
        str = MacroName;
        if (MacroArguments.Count > 0)
        {
            str += $"({string.Join(",", MacroArguments)})";
        }

        return Result.ImmutableOk();
    }

}
