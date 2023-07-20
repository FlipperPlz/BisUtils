namespace BisUtils.PreProcessor.RV.Models.Directives;

public interface IRVIfNotDefinedDirective : IRVIfDefinedDirective
{

}

public class RVIfNotDefinedDirective: RVIfDefinedDirective, IRVIfNotDefinedDirective
{
    public RVIfNotDefinedDirective(IRVPreProcessor processor, string macroName, string ifBody, string? elseBody = null) : base(processor, "ifndef", macroName, ifBody, elseBody)
    {
    }
}
