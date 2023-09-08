namespace BisUtils.RvShape.Models.Point;

using Core.Binarize.Flagging;
using Core.Extensions;
using Core.IO;
using Core.Render.Vector;
using FResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Options;

public interface IRvPoint : IVector3D, IBisFlaggable<RvPointFlag>
{
    bool IsHidden { get; set; }
}

public class RvPoint : BinarizableVector3D, IRvPoint
{
    public RvPointFlag Flags { get; set; }
    public bool IsHidden { get => this.HasFlag(RvPointFlag.SpecialHidden); set => this.AddFlag(RvPointFlag.SpecialHidden); }


    public RvPoint(float x, float y, float z, int? flags, ILogger? logger) : base(x, y, z, logger) => Flags = (RvPointFlag)(flags ?? 0) ;

    public RvPoint(float x, float y, float z, bool hidden, ILogger logger) : base(x, y, z, logger) => IsHidden = hidden;


    public RvPoint(BisBinaryReader reader, RvShapeOptions options, ILogger? logger) : base(reader, options, false, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RvPoint() : base(NullLogger.Instance)
    {

    }

    public RvPoint(ILogger? logger) : base(logger)
    {

    }

    public Result Binarize(BisBinaryWriter writer, RvShapeOptions options)
    {
        var result = base.Binarize(writer, options);
        if(Flags is { } flag)
        {
            writer.Write((uint) flag);
        }

        return result;
    }

    public Result Debinarize(BisBinaryReader reader, RvShapeOptions options)
    {
        var result = base.Debinarize(reader, options);
        if (options.ExtendedPoint)
        {
            Flags = (RvPointFlag) reader.ReadInt32();
        }
        return result;
    }


}
