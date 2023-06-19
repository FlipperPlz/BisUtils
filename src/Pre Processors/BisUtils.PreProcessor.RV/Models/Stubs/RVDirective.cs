namespace BisUtils.PreProcessor.RV.Models.Stubs;

public interface IRVDirective : IRVElement
{
    public string DirectiveName { get; }
}

public abstract class RVDirective : RVElement, IRVDirective
{
    protected RVDirective(IRVPreProcessor processor, string directiveName) : base(processor) =>
        DirectiveName = directiveName;

    public string DirectiveName { get; }
}
