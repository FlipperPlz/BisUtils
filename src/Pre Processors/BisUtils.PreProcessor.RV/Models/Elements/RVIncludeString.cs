namespace BisUtils.PreProcessor.RV.Models.Elements;

using System.Text;
using Core.Parsing;
using Enumerations;
using FResults;
using Lexer;
using Stubs;

public interface IRVIncludeString : IRVElement
{
    string Value { get; }
    RVStringType StringType { get; }
}

public class RVIncludeString: RVElement, IRVIncludeString
{
    public const int MaxIncludePathLength = 128;
    public string Value { get; set; }
    public RVStringType StringType { get; set; }

    public RVIncludeString(IRVPreProcessor preProcessor, string value, RVStringType stringType = RVStringType.Angled) : base(preProcessor)
    {
        Value = value;
        StringType = stringType;
    }

    public override Result ToText(out string str) => throw new NotImplementedException();

    public static Result ParseString(IRVPreProcessor processor, RVLexerOld lexerOld, out IRVIncludeString str)
    {
        lexerOld.TraverseWhitespace(out _);
        var stringType = DetectStringType(lexerOld);
        str = new RVIncludeString(processor, ReadString(lexerOld, stringType), stringType);
        return Result.ImmutableOk();
    }

    private static string ReadString(BisStringStepper lexer, RVStringType stringType)
    {
        var path = new StringBuilder();
        var suffix = SuffixFor(stringType);
        while (path.Length < MaxIncludePathLength && !lexer.IsEOF() && lexer.MoveForward() != suffix)
        {
            path.Append(lexer.CurrentChar);
        }

        return path.ToString();
    }

    private static char SuffixFor(RVStringType stringType) => stringType switch
    {
        RVStringType.Angled => '>',
        RVStringType.Quoted => '"',
        _ => throw new ArgumentOutOfRangeException("TODO")
    };

    private static RVStringType DetectStringType(BisStringStepper lexer) =>
        lexer.CurrentChar switch
        {
            '<' => RVStringType.Angled,
            '"' => RVStringType.Quoted,
            _ => throw new ArgumentOutOfRangeException("TODO")
        };
}
