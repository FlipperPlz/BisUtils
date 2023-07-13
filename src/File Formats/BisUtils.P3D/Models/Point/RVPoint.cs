namespace BisUtils.P3D.Models.Point;

using Core.Extensions;
using Core.IO;
using FResults;
using Options;
using Utils;

public interface IRVPoint : IRVVector<RVShapeOptions>
{
    RVPointFlag? PointFlags { get; set; }
}

public class RVPoint : RVVector<RVShapeOptions>, IRVPoint
{
    public RVPointFlag? PointFlags { get; set; }

    public RVPoint(float x, float y, float z, RVPointFlag? flags) : base(x, y, z) => PointFlags = flags;

    public RVPoint(BisBinaryReader reader, RVShapeOptions options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RVPoint()
    {

    }

    public sealed override Result Binarize(BisBinaryWriter writer, RVShapeOptions options)
    {
        var result = base.Binarize(writer, options);
        if(PointFlags is { } flag)
        {
            writer.Write((uint) flag);
        }

        return result;
    }

    public new Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        var result = base.Debinarize(reader, options);
        if (options.ExtendedPoint)
        {
            PointFlags = (RVPointFlag) reader.ReadUInt32();
        }
        return result;
    }
}
