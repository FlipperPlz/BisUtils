namespace BisUtils.Core.Render.Vector;

using Binarize.Implementation;
using Binarize.Options;
using Extensions;
using FResults;
using IO;
using Microsoft.Extensions.Logging;
using Point;

public interface IVector2D
{
    float X { get; }
    float Y { get; }
}

public readonly struct Vector2D : IVector2D
{
    public float X { get; }
    public float Y { get; }

    public Vector2D(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Vector2D(int x, int y) : this (Convert.ToSingle(x), Convert.ToSingle(y))
    {

    }
    public static implicit operator BinarizablePoint2D(Vector2D point) => new(point.X, point.Y, null!);
    public static explicit operator Vector2D(BinarizablePoint2D point) => new(point.X, point.Y);
}


public class BinarizableVector2D : BinaryObject<IBinarizationOptions>, IVector2D
{
    public float X { get; private set; }
    public float Y { get; private set; }

    public static implicit operator Vector2D(BinarizableVector2D point) => new(point.X, point.Y);
    public static explicit operator BinarizableVector2D(Vector2D point) => new(point.X, point.Y, null!);

    public BinarizableVector2D(int x, int y, ILogger? logger) : this((float)x, y, logger)
    {
    }

    public BinarizableVector2D(float x, float y, ILogger? logger) : base(logger)
    {
        X = x;
        Y = y;
    }

    // ReSharper disable once UnusedParameter.Local
    protected BinarizableVector2D(BisBinaryReader reader, IBinarizationOptions options, bool _, ILogger? logger) : base(reader, options, logger)
    {

    }

    public BinarizableVector2D(BisBinaryReader reader, IBinarizationOptions options, ILogger? logger) : base(reader, options, logger)
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
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        return LastResult = Result.Ok();
    }
}
