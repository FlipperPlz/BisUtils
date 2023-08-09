namespace BisUtils.RvShape.Errors;

using FResults.Reasoning;
using Models.Lod;

public class RvShapeFaceReadError : ErrorBase
{
    public sealed override string? AlertName { get; init; }
    public sealed override Type? AlertScope { get; init; }
    public sealed override string? Message { get; set; }

    public RvShapeFaceReadError(string reason, string alertName = "Generic Face Read Failure")
    {
        AlertName = alertName;
        AlertScope = typeof(RvLod);
        Message = $"There was an error reading a LOD Face. {reason}";
    }
}
