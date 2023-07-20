namespace BisUtils.RVShape.Models.Point;

using BisUtils.Core.Extensions;
using Core.IO;
using Core.Render.Vector;
using Options;
using Core.Binarize.Flagging;
using FResults;

public interface IRVPoint : IVector3D, IBisFlaggable<RVPointFlag>
{
    bool IsHidden { get; set; }
}

public class RVPoint : BinarizableVector3D, IRVPoint
{
    public RVPointFlag Flags { get; set; }
    public bool IsHidden { get => this.HasFlag(RVPointFlag.SpecialHidden); set => this.AddFlag(RVPointFlag.SpecialHidden); }


    public RVPoint(float x, float y, float z, int? flags) : base(x, y, z) => Flags = (RVPointFlag)(flags ?? 0) ;

    public RVPoint(float x, float y, float z, bool hidden) : base(x, y, z) => IsHidden = hidden;


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
        if(Flags is { } flag)
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
            Flags = (RVPointFlag) reader.ReadInt32();
        }
        return result;
    }


}
