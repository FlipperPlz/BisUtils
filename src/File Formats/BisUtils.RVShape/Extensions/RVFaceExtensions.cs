namespace BisUtils.P3D.Extensions;

using Models.Face;

public static class RVFaceExtensions
{
    public static int VertexCount(this IRVFace face) =>
        face.Vertices.Count;
}
