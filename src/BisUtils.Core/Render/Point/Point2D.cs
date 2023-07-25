namespace BisUtils.Core.Render.Point;

using Binarize.Implementation;
using Binarize.Options;
using Extensions;
using FResults;
using IO;
using Microsoft.Extensions.Logging;

public interface IPoint2D
{
    int X { get; }
    int Y { get; }
}

public readonly struct Point2D : IPoint2D
{
    public int X { get; }
    public int Y { get; }

    public Point2D(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point2D(float x, float y) : this(Convert.ToInt32(x), Convert.ToInt32(y))
    {
    }

    public static implicit operator BinarizablePoint2D(Point2D point) => new(point.X, point.Y, null!);
    public static explicit operator Point2D(BinarizablePoint2D point) => new(point.X, point.Y);
}


public class BinarizablePoint2D : BinaryObject<IBinarizationOptions>, IPoint2D
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public static implicit operator Point2D(BinarizablePoint2D point) => new(point.X, point.Y);
    public static explicit operator BinarizablePoint2D(Point2D point) => new(point.X, point.Y, null!);

    public BinarizablePoint2D(int x, int y, ILogger? logger) : base(logger)
    {
        X = x;
        Y = y;
    }

    public BinarizablePoint2D(float x, float y, ILogger? logger) : this(Convert.ToInt32(x), Convert.ToInt32(y), logger)
    {
    }

    // ReSharper disable once UnusedParameter.Local
    protected BinarizablePoint2D(BisBinaryReader reader, IBinarizationOptions options, ILogger? logger, bool _) : base(reader, options, logger)
    {

    }

    public BinarizablePoint2D(BisBinaryReader reader, IBinarizationOptions options, ILogger? logger) : base(reader, options, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }


    public override Result Binarize(BisBinaryWriter writer, IBinarizationOptions options)
    {
        writer.Write(X);
        writer.Write(Y);
        return LastResult = Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, IBinarizationOptions options)
    {
        X = reader.ReadInt32();
        Y = reader.ReadInt32();
        return LastResult = Result.Ok();
    }
}

