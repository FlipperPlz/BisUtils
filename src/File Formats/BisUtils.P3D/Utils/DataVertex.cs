namespace BisUtils.P3D.Utils;

public struct DataVertex
{
    public DataVertex(int point, int normal, float mapU, float mapV)
    {
        Point = point;
        Normal = normal;
        MapU = mapU;
        MapV = mapV;
    }

    public int Point { get; }
    public int Normal { get; }
    public float MapU { get; }
    public float MapV { get; }
}
