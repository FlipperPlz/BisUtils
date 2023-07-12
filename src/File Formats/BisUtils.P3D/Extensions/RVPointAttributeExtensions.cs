namespace BisUtils.P3D.Extensions;

using Models.Point;

public static class RVPointAttributeExtensions
{

    public static int Size<T>(this IRVPointAttrib<T> data) where T : struct => data.Attributes.Count;

}
