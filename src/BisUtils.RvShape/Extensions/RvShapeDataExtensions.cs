namespace BisUtils.RvShape.Extensions;

using Models.Lod;

public static class RvShapeDataExtensions
{
    public static int PointCount(this IRvLod data) => data.Points.Count;
    public static int NormalsCount(this IRvLod data) => data.Normals.Count;
    public static int FacesCount(this IRvLod data) => data.Faces.Count;


    public static void ResetMass(this IRvLod lod, int? index = null)
    {
        if (index is { } nIndex)
        {
            lod.Mass[nIndex] = 0;
            return;
        }
        for (var i = 0; i < lod.Mass.Size(); i++)
        {
            lod.Mass[i] = 0;
        }
    }
}
