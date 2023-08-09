namespace BisUtils.RvProcess.Models.Directives;

public interface IRvIfNotDefinedDirective : IRvIfDefinedDirective
{

}

public class RvIfNotDefinedDirective: RvIfDefinedDirective, IRvIfNotDefinedDirective
{
    public RvIfNotDefinedDirective(IRvPreProcessor processor, string macroName, string ifBody, string? elseBody = null) : base(processor, "ifndef", macroName, ifBody, elseBody)
    {
    }
}
