namespace BisUtils.Param.Errors;

using FResults.Reasoning;
using Models.Stubs.Holders;

public class ParamStatementNotFoundError : ErrorBase
{
    public override string? AlertName { get; init; }
    public override Type? AlertScope { get; init; }
    public sealed override string? Message { get; set; }

    //TODO context.path
    public ParamStatementNotFoundError(string name, IParamStatementHolder context) =>
        Message =
            $"There was an error finding a statement named {name}. Perhaps one doesn't exist in the called context.";
}
