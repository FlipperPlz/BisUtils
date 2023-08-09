namespace BisUtils.RvShape.Models.Data;

public interface IRvUvMap
{
    public float MapU { get; set; }
    public float MapV { get; set; }
}

public struct RvUvMap : IRvUvMap
{
    public float MapU { get; set; }
    public float MapV { get; set; }

    public RvUvMap(float u, float v)
    {
        MapU = u;
        MapV = v;
    }
}
