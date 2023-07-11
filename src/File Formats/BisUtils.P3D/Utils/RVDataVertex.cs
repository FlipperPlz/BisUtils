namespace BisUtils.P3D.Utils;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Binarize.Options;
using Core.Extensions;
using Core.IO;
using FResults;

public interface IRVDataVertex : IBinaryObject<IBinarizationOptions>
{
    public int Point { get; set; }
    public int Normal { get; set; }
    public float MapU { get; set; }
    public float MapV { get; set; }
}

public class RVDataVertex : BinaryObject<IBinarizationOptions>, IRVDataVertex
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

    public RVDataVertex(BisBinaryReader reader, IBinarizationOptions options) : base(reader, options)
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
