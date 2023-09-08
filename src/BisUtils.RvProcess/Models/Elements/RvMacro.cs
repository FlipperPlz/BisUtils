// namespace BisUtils.RvProcess.Models.Elements;
//
// using FResults;
// using Stubs;
//
// public interface IRvMacro : IRvElement
// {
//     string MacroName { get; }
//     List<string> MacroArguments { get; }
// }
//
// public class RvMacro : RvElement, IRvMacro
// {
//     public string MacroName { get; }
//     public List<string> MacroArguments { get; }
//
//     public RvMacro(IRvPreProcessor preProcessor, string macroName, List<string>? macroArguments = null) : base(preProcessor)
//     {
//         MacroName = macroName;
//         MacroArguments = macroArguments ?? new List<string>();
//     }
//
//     public override Result ToText(out string str)
//     {
//         str = MacroName;
//         if (MacroArguments.Count > 0)
//         {
//             str += $"({string.Join(",", MacroArguments)})";
//         }
//
//         return Result.ImmutableOk();
//     }
//
// }
