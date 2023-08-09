namespace BisUtils.RvShape.Extensions;

using Models.Point;

public static class RvPointAttributeExtensions
{

    public static int Size<T>(this IRvPointAttrib<T> data) where T : struct => data.Attributes.Count;

}
