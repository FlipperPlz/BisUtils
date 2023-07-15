namespace BisUtils.RVShape.Models.Point;

using BisUtils.Core.Extensions;
using BisUtils.Core.IO;
using BisUtils.Core.Render.Vector;
using BisUtils.RVShape.Options;
using FResults;

public interface IRVPoint : IVector3D
{
    int? PointFlags { get; set; }
}

public class RVPoint : BinarizableVector3D, IRVPoint
{
    public int? PointFlags { get; set; }

    public RVPoint(float x, float y, float z, int? flags) : base(x, y, z) => PointFlags = flags;


    public RVPoint(BisBinaryReader reader, RVShapeOptions options) : base(reader, options, false)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RVPoint()
    {

    }

    public Result Binarize(BisBinaryWriter writer, RVShapeOptions options)
    {
        var result = base.Binarize(writer, options);
        if(PointFlags is { } flag)
        {
            writer.Write((uint) flag);
        }

        return result;
    }

    public Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        var result = base.Debinarize(reader, options);
        if (options.ExtendedPoint)
        {
            PointFlags = reader.ReadInt32();
        }
        return result;
    }
}
