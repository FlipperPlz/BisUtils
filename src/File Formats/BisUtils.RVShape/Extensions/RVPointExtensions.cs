namespace BisUtils.RVShape.Extensions;

using BisUtils.RVShape.Models.Point;

public static class RVPointExtensions
{
    public static bool IsExtended(this IRVPoint point) =>
        point.PointFlags is not null;
}
