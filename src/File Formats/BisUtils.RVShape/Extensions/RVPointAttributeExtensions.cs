namespace BisUtils.RVShape.Extensions;

using BisUtils.RVShape.Models.Point;

public static class RVPointAttributeExtensions
{

    public static int Size<T>(this IRVPointAttrib<T> data) where T : struct => data.Attributes.Count;

}
