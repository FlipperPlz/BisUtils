namespace BisUtils.RvShape.Models.Utils;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using Data;
using FResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Options;

public interface IRvDataVertex : IBinaryObject<RvShapeOptions>, IRvUvMap
{
    public int Point { get; set; }
    public int Normal { get; set; }
    public IRvUvMap? FaceUV { get; set; }
}

public class RvDataVertex : BinaryObject<RvShapeOptions>, IRvDataVertex
{
    public int Point { get; set; }
    public int Normal { get; set; }
    public float MapU { get; set; }
    public float MapV { get; set;  }
    public IRvUvMap? FaceUV { get; set; }

    public RvDataVertex(int point, int normal, float mapU, float mapV, IRvUvMap? uvMap, ILogger? logger) : base(logger)
    {
        FaceUV = uvMap;
        Point = point;
        Normal = normal;
        MapU = mapU;
        MapV = mapV;
    }

    public RvDataVertex(ILogger? logger) : base(logger)
    {

    }

    public RvDataVertex() : base(NullLogger.Instance)
    {

    }

    public RvDataVertex(BisBinaryReader reader, RvShapeOptions options, ILogger? logger) : base(reader, options, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, RvShapeOptions options)
    {
        writer.Write(Point);
        writer.Write(Normal);
        writer.Write(MapU);
        writer.Write(MapV);
        return Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RvShapeOptions options)
    {
        Point = reader.ReadInt32();
        Normal = reader.ReadInt32();
        MapU = reader.ReadSingle();
        MapV = reader.ReadSingle();
        return Result.Ok();
    }

}
