// namespace BisUtils.RvProcess.Models.Elements;
//
// using System.Text;
// using Enumerations;
// using FResults;
// using Stubs;
//
// public interface IRvIncludeString : IRvElement
// {
//     string Value { get; }
//     RvStringType StringType { get; }
// }
//
// public class RvIncludeString: RvElement, IRvIncludeString
// {
//     public const int MaxIncludePathLength = 128;
//     public string Value { get; set; }
//     public RvStringType StringType { get; set; }
//
//     public RvIncludeString(IRvPreProcessor preProcessor, string value, RvStringType stringType = RvStringType.Angled) : base(preProcessor)
//     {
//         Value = value;
//         StringType = stringType;
//     }
//
//     public override Result ToText(out string str) => throw new NotImplementedException();
//
//
//
//     private static string ReadString(BisStringStepper lexer, RvStringType stringType)
//     {
//         var path = new StringBuilder();
//         var suffix = SuffixFor(stringType);
//         while (path.Length < MaxIncludePathLength && !lexer.IsEOF() && lexer.MoveForward() != suffix)
//         {
//             path.Append(lexer.CurrentChar);
//         }
//
//         return path.ToString();
//     }
//
//     private static char SuffixFor(RvStringType stringType) => stringType switch
//     {
//         RvStringType.Angled => '>',
//         RvStringType.Quoted => '"',
//         _ => throw new ArgumentOutOfRangeException("TODO")
//     };
//
//     private static RvStringType DetectStringType(BisStringStepper lexer) =>
//         lexer.CurrentChar switch
//         {
//             '<' => RvStringType.Angled,
//             '"' => RvStringType.Quoted,
//             _ => throw new ArgumentOutOfRangeException("TODO")
//         };
// }
