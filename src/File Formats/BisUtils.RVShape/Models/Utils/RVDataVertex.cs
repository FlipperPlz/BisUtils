namespace BisUtils.P3D.Models.Utils;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using FResults;
using Options;

public interface IRVDataVertex : IBinaryObject<RVShapeOptions>
{
    public int Point { get; set; }
    public int Normal { get; set; }
    public float MapU { get; set; }
    public float MapV { get; set; }
}

public class RVDataVertex : BinaryObject<RVShapeOptions>, IRVDataVertex
{
    public int Point { get; set; }
    public int Normal { get; set; }
    public float MapU { get; set; }
    public float MapV { get; set;  }

    public RVDataVertex(int point, int normal, float mapU, float mapV)
    {
        Point = point;
        Normal = normal;
        MapU = mapU;
        MapV = mapV;
    }

    public RVDataVertex()
    {

    }

    public RVDataVertex(BisBinaryReader reader, RVShapeOptions options) : base(reader, options)
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
