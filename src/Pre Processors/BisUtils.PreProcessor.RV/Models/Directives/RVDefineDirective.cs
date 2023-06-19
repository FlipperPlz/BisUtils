namespace BisUtils.PreProcessor.RV.Models.Directives;

using System.Text;
using System.Text.RegularExpressions;
using FResults;
using Stubs;

public interface IRVDefineDirective : IRVDirective
{
    string MacroName { get; }
    string? MacroValue { get; }
    List<string> MacroArguments { get; }
    string Evaluate(List<string> arguments);
}

public class RVDefineDirective : RVDirective, IRVDefineDirective
{
    public string MacroName { get; set; }
    public string? MacroValue { get; set; }
    public List<string> MacroArguments { get; set; }

    public RVDefineDirective
    (
        IRVPreProcessor processor,
        string macroName,
        string? macroValue = null,
        List<string>? macroArguments = null
    ) : base(processor, "define")
    {
        MacroName = macroName;
        MacroValue = macroValue;
        MacroArguments = macroArguments ?? new List<string>();
    }

    public string Evaluate(List<string> arguments)
    {
        if (string.IsNullOrWhiteSpace(MacroValue))
        {
            return "1";
        }

        return Regex.Replace(MacroValue, @"\b(" + string.Join("|", MacroArguments) + @")\b", matchResult =>
        {
            var argumentIndex = MacroArguments.IndexOf(matchResult.Value);
            return argumentIndex >= 0 && argumentIndex < arguments.Count ? arguments[argumentIndex] : "";
        });
    }

    public override Result ToText(out string str)
    {
        str = $"{DirectiveKeyword} {MacroName}";
        if (MacroArguments.Any())
        {
            str += $"({string.Join(", ", MacroArguments)}";
        }

        if (!string.IsNullOrWhiteSpace(MacroValue))
        {
            str += $" {MacroValue}";
        }

        return Result.ImmutableOk();
    }
}
