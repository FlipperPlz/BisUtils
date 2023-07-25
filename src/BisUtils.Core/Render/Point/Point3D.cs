namespace BisUtils.Core.Render.Point;

using Binarize.Implementation;
using Binarize.Options;
using Extensions;
using FResults;
using IO;
using Microsoft.Extensions.Logging;

public interface IPoint3D : IPoint2D
{
    int Z { get; }
}

public readonly struct Point3D : IPoint3D
{
    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public Point3D(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Point3D(float x, float y, float z) : this(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(z))
    {
    }

    public static implicit operator BinarizablePoint3D(Point3D point) => new(point.X, point.Y, point.Z, null!);
    public static explicit operator Point3D(BinarizablePoint3D point) => new(point.X, point.Y, point.Z);
}


public class BinarizablePoint3D : BinaryObject<IBinarizationOptions>, IPoint3D
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }

    public static implicit operator Point3D(BinarizablePoint3D point) => new(point.X, point.Y, point.Z);
    public static explicit operator BinarizablePoint3D(Point3D point) => new(point.X, point.Y, point.Z, null);

    public BinarizablePoint3D(int x, int y, int z, ILogger? logger) : base(logger)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public BinarizablePoint3D(float x, float y, float z, ILogger? logger) : this(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(z), logger)
    {
    }

    // ReSharper disable once UnusedParameter.Local
    protected BinarizablePoint3D(BisBinaryReader reader, IBinarizationOptions options, ILogger? logger, bool _) : base(reader, options, logger)
    {

    }

    public BinarizablePoint3D(BisBinaryReader reader, IBinarizationOptions options, ILogger? logger) : base(reader, options, logger)
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
        writer.Write(Z);
        return LastResult = Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, IBinarizationOptions options)
    {
        X = reader.ReadInt32();
        Y = reader.ReadInt32();
        Z = reader.ReadInt32();
        return LastResult = Result.Ok();
    }
}

