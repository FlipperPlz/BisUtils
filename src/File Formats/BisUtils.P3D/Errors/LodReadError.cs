namespace BisUtils.P3D.Errors;

using FResults.Reasoning;
using Models;

public class LodReadError : ErrorBase
{
    public sealed override string? AlertName { get; init; }
    public sealed override Type? AlertScope { get; init; }
    public sealed override string? Message { get; set; }

    public LodReadError(string reason, string alertName = "Generic Lod Debinarization Failure")
    {
        AlertName = alertName;
        AlertScope = typeof(RVLod);
        Message = $"There was an error reading a shape LOD. {reason}";
    }
}
