namespace BisUtils.RVShape.Models.Data;

public interface IRVUvMap
{
    public float MapU { get; set; }
    public float MapV { get; set; }
}

public struct RVUvMap : IRVUvMap
{
    public float MapU { get; set; }
    public float MapV { get; set; }

    public RVUvMap(float u, float v)
    {
        MapU = u;
        MapV = v;
    }
}
