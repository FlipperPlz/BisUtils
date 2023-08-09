namespace BisUtils.RvProcess.Models.Directives;

using Elements;
using FResults;
using Stubs;

public interface IRvIncludeDirective : IRvDirective
{
    IRvIncludeString IncludeTarget { get; }
}


public class RvIncludeDirective : RvDirective, IRvIncludeDirective
{
    public IRvIncludeString IncludeTarget { get; set; }

    public RvIncludeDirective(IRvPreProcessor processor, IRvIncludeString includeTarget) : base(processor, "include") =>
        IncludeTarget = includeTarget;

    public override Result ToText(out string str) => throw new NotImplementedException();
}
