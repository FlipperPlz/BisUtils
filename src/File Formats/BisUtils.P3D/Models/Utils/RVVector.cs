namespace BisUtils.P3D.Models.Utils;

using Core.Binarize;
using Core.Binarize.Implementation;
using BisUtils.Core.Binarize.Options;
using Core.Extensions;
using Core.IO;
using FResults;

public interface IRVVector : IRVVector<IBinarizationOptions>
{

}
public interface IRVVector<in TOptions>: IBinaryObject<TOptions> where TOptions : IBinarizationOptions
{
    float X { get; set; }
    float Y { get; set; }
    float Z { get; set; }
    float SquaredSize();

    static IRVVector<TOptions> operator -(IRVVector<TOptions> a, IRVVector<TOptions> b) =>
        new RVVector<TOptions>(a.X-b.X, a.Y-b.Y, a.Z-b.Z);

    static IRVVector<TOptions> operator /(float a, IRVVector<TOptions> b)
    {
        var i = 1 / a;
        return new RVVector<TOptions>(b.X*i, b.Y*i, b.Z*i);
    }

}

public class RVVector : RVVector<IBinarizationOptions>, IRVVector
{
    public RVVector()
    {
    }

    public RVVector(float x, float y, float z) : base(x, y, z)
    {
    }

    public RVVector(BisBinaryReader reader, IBinarizationOptions options) : base(reader, options)
    {
    }
}
public class RVVector<TOptions>: BinaryObject<TOptions>, IRVVector<TOptions> where TOptions : IBinarizationOptions
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public float SquaredSize() => (X*X)+(Y*Y)+(Z*Z);

    public RVVector(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public RVVector()
    {

    }

    public RVVector(BisBinaryReader reader, TOptions options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }


    public override Result Binarize(BisBinaryWriter writer, TOptions options)
    {
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Z);
        return Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, TOptions options)
    {
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();
        return Result.Ok();
    }
}

