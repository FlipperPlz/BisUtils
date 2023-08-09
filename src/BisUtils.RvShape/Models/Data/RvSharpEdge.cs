namespace BisUtils.RvShape.Models.Data;

using Core.Binarize;
using Core.Extensions;
using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IRvSharpEdge
{
    int EdgeX { get; set; }
    int EdgeY { get; }
    int this[int index] => index == 0 ? EdgeX : EdgeY;
    void OrganizeEdges();
}


public struct RvSharpEdge : IRvSharpEdge, IBinaryObject<RvShapeOptions>
{
    public int EdgeX { get; set; }
    public int EdgeY { get; set; }
    public ILogger? Logger { get; }

    public static int CompareEdges(IRvSharpEdge edge0, IRvSharpEdge edge1)
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

    public RvSharpEdge(int edgeX, int edgeY, ILogger logger)
    {
        EdgeX = edgeX;
        EdgeY = edgeY;
        Logger = logger;
    }

    public RvSharpEdge(BisBinaryReader reader, RvShapeOptions options, ILogger logger)
    {
        Logger = logger;
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public Result Binarize(BisBinaryWriter writer, RvShapeOptions options)
    {
        writer.Write(EdgeX);
        writer.Write(EdgeY);
        return LastResult = Result.Ok();
    }

    public Result Debinarize(BisBinaryReader reader, RvShapeOptions options)
    {
        EdgeX = reader.ReadInt32();
        EdgeY = reader.ReadInt32();
        return LastResult = Result.Ok();
    }

}
