namespace BisUtils.P3D.Extensions;

using Models.Data;

public static class RVShapeDataExtensions
{
    public static int PointCount(this IRVShapeData data) => data.Points.Count;
    public static int NormalsCount(this IRVShapeData data) => data.Normals.Count;
    public static int FacesCount(this IRVShapeData data) => data.Faces.Count;

}
