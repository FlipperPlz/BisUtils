namespace BisUtils.RVShape.Models.Data;

using BisUtils.Core.Binarize;
using BisUtils.Core.Binarize.Implementation;
using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Extensions;
using BisUtils.Core.IO;
using BisUtils.Core.Render.Vector;
using BisUtils.RVShape.Models.Lod;
using BisUtils.RVShape.Options;
using FResults;

public interface IRVAnimationPhase : IBinaryObject<RVShapeOptions>
{
    RVLod Parent { get; }
    List<IVector3D> Points { get; }
    float Time { get; }
}

public class RVAnimationPhase : BinaryObject<RVShapeOptions>, IRVAnimationPhase
{
    public required RVLod Parent { get; init; }
    public List<IVector3D> Points { get; private set; } = null!;
    public float Time { get; set;  }

    public RVAnimationPhase(BisBinaryReader reader, RVShapeOptions options) : base(reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RVAnimationPhase(float time, List<IVector3D> points)
    {
        Points = points;
        Time = time;
    }

    public IVector3D? this[int i]
    {
        get => Points.Count < i ? null : Points[i];
        set
        {
            if (value is { } notnull)
            {
                Points[i] = notnull;
            }
        }
    }

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options)
    {
        writer.Write(Time);
        Points.Cast<BinarizableVector3D>().WriteBinarized(writer, options);
        return LastResult = Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        Time = reader.ReadSingle();
        Points = reader.ReadIndexedList<BinarizableVector3D, IBinarizationOptions>(options, Parent.Points.Count)
            .Cast<IVector3D>()
            .ToList();
        return LastResult = Result.Ok();
    }
}
