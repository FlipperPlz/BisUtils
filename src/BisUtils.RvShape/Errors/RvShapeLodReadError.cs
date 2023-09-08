namespace BisUtils.RvShape.Errors;

using FResults.Reasoning;
using Models.Lod;

public class RvShapeLodReadError : ErrorBase
{
    public sealed override string? AlertName { get; init; }
    public sealed override Type? AlertScope { get; init; }
    public sealed override string? Message { get; set; }

    public RvShapeLodReadError(string reason, string alertName = "Generic Lod Read Failure")
    {
        AlertName = alertName;
        AlertScope = typeof(RvLod);
        Message = $"There was an error reading a shape LOD. {reason}";
    }
}
