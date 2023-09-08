namespace BisUtils.RvShape.Extensions;

using Models.Face;

public static class RvFaceExtensions
{
    public static int VertexCount(this IRvFace face) =>
        face.Vertices.Count;
}
