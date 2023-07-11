namespace BisUtils.P3D.Models.Point;

using Core.Binarize.Options;
using Core.Extensions;
using Core.IO;
using FResults;

public interface IRVPoint : IRVVector
{
    RVPointOptions PointFlags { get; }
}

public class RVPoint : RVVector, IRVPoint
{
    public RVPointOptions PointFlags { get; private set; }

    public RVPoint(float x, float y, float z) : base(x, y, z)
    {
    }

    public RVPoint(BisBinaryReader reader, IBinarizationOptions options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, IBinarizationOptions options)
    {
        var result = base.Binarize(writer, options);
        writer.Write((uint) PointFlags);
        return result;
    }

    public new Result Debinarize(BisBinaryReader reader, IBinarizationOptions options)
    {
        var result = base.Debinarize(reader, options);
        PointFlags = (RVPointOptions) reader.ReadUInt32();
        return result;
    }
}
