namespace BisUtils.P3D.Models.Data;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Binarize.Options;
using Core.Extensions;
using Core.IO;
using Core.Render.Vector;
using FResults;
using Lod;
using Options;

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
