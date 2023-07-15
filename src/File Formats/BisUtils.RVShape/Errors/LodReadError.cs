namespace BisUtils.RVShape.Errors;

using BisUtils.RVShape.Models.Lod;
using FResults.Reasoning;

public class LodReadError : ErrorBase
{
    public sealed override string? AlertName { get; init; }
    public sealed override Type? AlertScope { get; init; }
    public sealed override string? Message { get; set; }

    public LodReadError(string reason, string alertName = "Generic Lod Read Failure")
    {
        AlertName = alertName;
        AlertScope = typeof(RVLod);
        Message = $"There was an error reading a shape LOD. {reason}";
    }
}
