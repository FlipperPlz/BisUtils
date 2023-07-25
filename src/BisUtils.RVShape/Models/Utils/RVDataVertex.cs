namespace BisUtils.RVShape.Models.Utils;

using Core.Binarize;
using Core.Binarize.Implementation;
using BisUtils.Core.Extensions;
using Core.IO;
using Data;
using Options;
using FResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public interface IRVDataVertex : IBinaryObject<RVShapeOptions>, IRVUvMap
{
    public int Point { get; set; }
    public int Normal { get; set; }
    public IRVUvMap? FaceUV { get; set; }
}

public class RVDataVertex : BinaryObject<RVShapeOptions>, IRVDataVertex
{
    public int Point { get; set; }
    public int Normal { get; set; }
    public float MapU { get; set; }
    public float MapV { get; set;  }
    public IRVUvMap? FaceUV { get; set; }

    public RVDataVertex(int point, int normal, float mapU, float mapV, IRVUvMap? uvMap, ILogger? logger) : base(logger)
    {
        FaceUV = uvMap;
        Point = point;
        Normal = normal;
        MapU = mapU;
        MapV = mapV;
    }

    public RVDataVertex(ILogger? logger) : base(logger)
    {

    }

    public RVDataVertex() : base(NullLogger.Instance)
    {

    }

    public RVDataVertex(BisBinaryReader reader, RVShapeOptions options, ILogger? logger) : base(reader, options, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options)
    {
        writer.Write(Point);
        writer.Write(Normal);
        writer.Write(MapU);
        writer.Write(MapV);
        return Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        Point = reader.ReadInt32();
        Normal = reader.ReadInt32();
        MapU = reader.ReadSingle();
        MapV = reader.ReadSingle();
        return Result.Ok();
    }

}
