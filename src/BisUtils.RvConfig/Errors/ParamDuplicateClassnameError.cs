namespace BisUtils.RvConfig.Errors;

using FResults.Reasoning;
using Models.Statements;
using Models.Stubs.Holders;

public class ParamDuplicateClassnameError : ErrorBase
{
    public override string? AlertName { get; init; }
    public override Type? AlertScope { get; init; }
    public sealed override string? Message { get; set; }

    //TODO context.path, line numbering
    public ParamDuplicateClassnameError(IParamExternalClass first, IParamExternalClass duplicate) =>
        Message = $"There seems to be two classes under the name {first.ClassName}.";
}
