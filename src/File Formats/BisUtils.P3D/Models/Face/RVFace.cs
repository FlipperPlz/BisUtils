namespace BisUtils.P3D.Models.Face;

using BisUtils.P3D.Utils;

public interface IRVFace
{
    const int MaxDataCount = 4;
    string Texture { get; }
    string Material { get; }
    IEnumerable<DataVertex> Vertices { get; }
}

public struct RVFace : IRVFace
{
    public string Texture { get; }
    public string Material { get; }
    public IEnumerable<DataVertex> Vertices { get; }

    public RVFace(string texture, string material, IEnumerable<DataVertex> vertices)
    {
        Texture = texture;
        Material = material;
        Vertices = vertices;
    }
}


