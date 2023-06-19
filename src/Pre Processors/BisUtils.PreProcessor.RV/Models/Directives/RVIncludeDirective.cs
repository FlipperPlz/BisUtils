namespace BisUtils.PreProcessor.RV.Models.Directives;

using Elements;
using FResults;
using Lexer;
using Stubs;

public interface IRVIncludeDirective : IRVDirective
{
    IRVIncludeString IncludeTarget { get; }
}


public class RVIncludeDirective : RVDirective, IRVIncludeDirective
{
    public IRVIncludeString IncludeTarget { get; set; }

    public RVIncludeDirective(IRVPreProcessor processor, IRVIncludeString includeTarget) : base(processor, "include") =>
        IncludeTarget = includeTarget;

    public override Result ToText(out string str) => throw new NotImplementedException();

    public static Result ParseDirective(IRVPreProcessor processor, RVLexer lexer, out IRVIncludeDirective include)
    {
        var result = RVIncludeString.ParseString(processor, lexer, out var includeString);
        include = new RVIncludeDirective(processor, includeString);
        return result;
    }

    public override Result Process(RVLexer lexer, int startPosition) => throw new NotImplementedException();
}
