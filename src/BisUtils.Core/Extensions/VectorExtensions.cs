namespace BisUtils.Core.Extensions;

using Render.Vector;

public static class VectorExtensions
{
    public static float SquaredSize(this IVector3D vector) => (vector.X*vector.X)+(vector.Y*vector.Y)+(vector.Z*vector.Z);

}
