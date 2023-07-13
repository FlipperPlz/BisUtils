namespace BisUtils.P3D.Errors;

using FResults.Reasoning;
using Models;
using Models.Data;
using Models.Lod;

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
