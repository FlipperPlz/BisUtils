namespace BisUtils.RVShape.Extensions;

using BisUtils.RVShape.Models.Face;

public static class RVFaceExtensions
{
    public static int VertexCount(this IRVFace face) =>
        face.Vertices.Count;
}
