namespace BisUtils.RvShape.Models.Data;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Binarize.Options;
using Core.Extensions;
using Core.IO;
using Core.Render.Vector;
using FResults;
using Lod;
using Microsoft.Extensions.Logging;
using Options;

public interface IRvAnimationPhase : IBinaryObject<RvShapeOptions>
{
    RvLod Parent { get; }
    List<IVector3D> Points { get; }
    float Time { get; }
}

public class RvAnimationPhase : BinaryObject<RvShapeOptions>, IRvAnimationPhase
{
    public required RvLod Parent { get; init; }
    public List<IVector3D> Points { get; private set; } = null!;
    public float Time { get; set;  }

    public RvAnimationPhase(BisBinaryReader reader, RvShapeOptions options, ILogger? logger) : base(reader, options, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RvAnimationPhase(float time, List<IVector3D> points, ILogger? logger) : base(logger)
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

    public override Result Binarize(BisBinaryWriter writer, RvShapeOptions options)
    {
        writer.Write(Time);
        Points.Cast<BinarizableVector3D>().WriteBinarized(writer, options);
        return LastResult = Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RvShapeOptions options)
    {
        Time = reader.ReadSingle();
        Points = reader.ReadIndexedList<BinarizableVector3D, IBinarizationOptions>(options, Parent.Points.Count)
            .Cast<IVector3D>()
            .ToList();
        return LastResult = Result.Ok();
    }
}
