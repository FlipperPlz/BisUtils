namespace BisUtils.P3D.Models.Face;

using Utils;

public interface IRVExtendedFace : IRVFace
{
    ulong Flags { get; }
}


public struct RVExtendedFace : IRVExtendedFace
{
    public RVExtendedFace(ulong flags, string texture, string material, IEnumerable<DataVertex> vertices)
    {
        Flags = flags;
        Texture = texture;
        Material = material;
        Vertices = vertices;
    }

    public string Texture { get; }
    public string Material { get; }
    public ulong Flags { get; }
    public IEnumerable<DataVertex> Vertices { get; }
}
