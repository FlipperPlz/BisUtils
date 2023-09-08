// namespace BisUtils.RvProcess.Models.Directives;
//
// using FResults;
// using Stubs;
//
// public interface IRvUndefineDirective : IRvDirective
// {
//     string MacroName { get; }
// }
//
// public class RvUndefineDirective: RvDirective, IRvUndefineDirective
// {
//     public string MacroName { get; set; }
//
//     public RvUndefineDirective(IRvPreProcessor processor, string macroName) : base(processor, "undef") =>
//         MacroName = macroName;
//
//     public override Result ToText(out string str)
//     {
//         str = MacroName;
//         return Result.ImmutableOk();
//     }
// }
