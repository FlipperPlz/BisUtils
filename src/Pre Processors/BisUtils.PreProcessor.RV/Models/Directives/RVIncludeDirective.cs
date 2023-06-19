namespace BisUtils.PreProcessor.RV.Models.Directives;

using Elements;
using FResults;
using Stubs;

public interface IRVIncludeDirective : IRVDirective
{
    IRVIncludeString IncludeTarget { get; }
}


public class RVIncludeDirective : RVDirective, IRVIncludeDirective
{
    public IRVIncludeString IncludeTarget { get; set; }

    public RVIncludeDirective(IRVPreProcessor processor, string directiveName, IRVIncludeString includeTarget) : base(processor, directiveName) => IncludeTarget = includeTarget;

    public override Result ToText(out string str) => throw new NotImplementedException();
}
