namespace BisUtils.PreProcessor.RV.Models.Stubs;

using FResults;
using Lexer;

public interface IRVDirective : IRVElement
{
    public string DirectiveName { get; }

    Result Process(RVLexer lexer, int startPosition);
}

public abstract class RVDirective : RVElement, IRVDirective
{
    public string DirectiveName { get; }
    public string DirectiveKeyword { get; }

    protected RVDirective(IRVPreProcessor processor, string directiveName) : base(processor)
    {
        DirectiveName = directiveName;
        DirectiveKeyword = $"#{DirectiveName}";
    }

    public abstract Result Process(RVLexer lexer, int startPosition);

}
