namespace BisUtils.P3D.Models.Data;

public interface IRVSharpEdge
{
    int EdgeX { get; }
    int EdgeY { get; }
    int this[int index] => index == 0 ? EdgeX : EdgeY;
}

public struct RVSharpEdge : IRVSharpEdge
{
    public int EdgeX { get; }
    public int EdgeY { get; }

    public RVSharpEdge(int edgeX, int edgeY)
    {
        EdgeX = edgeX;
        EdgeY = edgeY;
    }
}
