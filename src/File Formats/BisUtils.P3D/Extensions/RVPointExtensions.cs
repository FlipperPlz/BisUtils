namespace BisUtils.P3D.Extensions;

using Models.Point;

public static class RVPointExtensions
{
    public static bool IsExtended(this IRVPoint point) =>
        point.PointFlags is not null;
}
