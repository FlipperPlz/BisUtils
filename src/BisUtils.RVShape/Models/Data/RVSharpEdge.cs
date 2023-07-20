namespace BisUtils.RVShape.Models.Data;

using BisUtils.Core.Binarize;
using BisUtils.Core.Extensions;
using BisUtils.Core.IO;
using BisUtils.RVShape.Options;
using FResults;

public interface IRVSharpEdge
{
    int EdgeX { get; set; }
    int EdgeY { get; }
    int this[int index] => index == 0 ? EdgeX : EdgeY;
    void OrganizeEdges();
}


public struct RVSharpEdge : IRVSharpEdge, IBinaryObject<RVShapeOptions>
{
    public int EdgeX { get; set; }
    public int EdgeY { get; set; }

    public static int CompareEdges(IRVSharpEdge edge0, IRVSharpEdge edge1)
    {
        var ret = edge0.EdgeX - edge1.EdgeX;
        if (ret != 0)
        {
            return ret;
        }

        return edge0.EdgeY - edge1.EdgeY;
    }


    public void OrganizeEdges()
    {
        if (EdgeX > EdgeY)
        {
            (EdgeX, EdgeY) = (EdgeY, EdgeX);
        }
    }

    public Result? LastResult { get; private set; }

    public RVSharpEdge(int edgeX, int edgeY)
    {
        EdgeX = edgeX;
        EdgeY = edgeY;
    }

    public RVSharpEdge(BisBinaryReader reader, RVShapeOptions options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public Result Binarize(BisBinaryWriter writer, RVShapeOptions options)
    {
        writer.Write(EdgeX);
        writer.Write(EdgeY);
        return LastResult = Result.Ok();
    }

    public Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        EdgeX = reader.ReadInt32();
        EdgeY = reader.ReadInt32();
        return LastResult = Result.Ok();
    }
}
