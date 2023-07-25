namespace BisUtils.Core.Render.Vector;

using Binarize.Implementation;
using Binarize.Options;
using Extensions;
using FResults;
using IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public interface IVector3D : IVector2D
{
    float Z { get; }
}

public readonly struct Vector3D : IVector3D
{
    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public static Vector3D operator -(Vector3D a, Vector3D b) =>
        new(a.X-b.X, a.Y-b.Y, a.Z-b.Z);

    public static Vector3D operator -(float a, Vector3D b)
    {
        var i = 1 / a;
        return new Vector3D(b.X*i, b.Y*i, b.Z*i);
    }

    public Vector3D(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3D(int x, int y, int z) : this(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z))
    {
    }
    public static implicit operator BinarizableVector3D(Vector3D point) => new(point.X, point.Y, point.Z, null);
    public static explicit operator Vector3D(BinarizableVector3D point) => new(point.X, point.Y, point.Z);
}

public class BinarizableVector3D : BinaryObject<IBinarizationOptions>, IVector3D
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Z { get; private set; }

    public static implicit operator Vector3D(BinarizableVector3D point) => new(point.X, point.Y, point.Z);
    public static explicit operator BinarizableVector3D(Vector3D point) => new(point.X, point.Y, point.Z, null!);

    public BinarizableVector3D(float x, float y, float z, ILogger? logger) : base(logger)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public BinarizableVector3D() : base(NullLogger.Instance)
    {

    }

    public BinarizableVector3D(ILogger? logger) : base(logger)
    {

    }

    // ReSharper disable once UnusedParameter.Local
    protected BinarizableVector3D(BisBinaryReader reader, IBinarizationOptions options, bool _, ILogger? logger) : base(reader, options, logger)
    {

    }

    public BinarizableVector3D(BisBinaryReader reader, IBinarizationOptions options, ILogger? logger) : base(reader, options, logger)
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
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();
        return LastResult = Result.Ok();
    }
}

