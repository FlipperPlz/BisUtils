namespace BisUtils.RVShape.Errors;

using BisUtils.RVShape.Models.Lod;
using FResults.Reasoning;

public class RVShapeFaceReadError : ErrorBase
{
    public sealed override string? AlertName { get; init; }
    public sealed override Type? AlertScope { get; init; }
    public sealed override string? Message { get; set; }

    public RVShapeFaceReadError(string reason, string alertName = "Generic Face Read Failure")
    {
        AlertName = alertName;
        AlertScope = typeof(RVLod);
        Message = $"There was an error reading a LOD Face. {reason}";
    }
}
