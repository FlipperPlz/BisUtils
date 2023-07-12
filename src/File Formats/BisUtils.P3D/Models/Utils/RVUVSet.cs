namespace BisUtils.P3D.Models.Utils;

using Core.Binarize;
using Core.Binarize.Implementation;
using BisUtils.Core.Binarize.Options;
using Core.Extensions;
using Core.IO;
using FResults;

public interface IRVUVSet : IBinaryObject<IBinarizationOptions>
{
    public int Point { get; set; }
    public int Normal { get; set; }
    public float MapU { get; set; }
    public float MapV { get; set; }
}

public class RVUVSet : BinaryObject<IBinarizationOptions>, IRVUVSet
{
    public int Point { get; set; }
    public int Normal { get; set; }
    public float MapU { get; set; }
    public float MapV { get; set;  }

    public RVUVSet(int point, int normal, float mapU, float mapV)
    {
        Point = point;
        Normal = normal;
        MapU = mapU;
        MapV = mapV;
    }

    public RVUVSet()
    {

    }

    public RVUVSet(BisBinaryReader reader, IBinarizationOptions options) : base(reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, IBinarizationOptions options)
    {
        writer.Write(Point);
        writer.Write(Normal);
        writer.Write(MapU);
        writer.Write(MapV);
        return Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, IBinarizationOptions options)
    {
        Point = reader.ReadInt32();
        Normal = reader.ReadInt32();
        MapU = reader.ReadSingle();
        MapV = reader.ReadSingle();
        return Result.Ok();
    }

}
