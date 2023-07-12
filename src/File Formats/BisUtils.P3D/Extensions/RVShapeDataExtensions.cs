namespace BisUtils.P3D.Extensions;

using Models.Data;

public static class RVShapeDataExtensions
{
    public static int PointCount(this IRVShapeData data) => data.Points.Count;
    public static int NormalsCount(this IRVShapeData data) => data.Normals.Count;
    public static int FacesCount(this IRVShapeData data) => data.Faces.Count;


    public static void ResetMass(this IRVShapeData lod, int? index = null)
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
