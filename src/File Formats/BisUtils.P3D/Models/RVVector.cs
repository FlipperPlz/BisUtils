namespace BisUtils.P3D.Models;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Binarize.Options;
using Core.Extensions;
using Core.IO;
using FResults;

public interface IRVVector: IBinaryObject<IBinarizationOptions>
{
    float X { get; set; }
    float Y { get; set; }
    float Z { get; set; }
}

public class RVVector: BinaryObject<IBinarizationOptions>, IRVVector
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public RVVector(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    protected RVVector()
    {

    }

    public RVVector(BisBinaryReader reader, IBinarizationOptions options)
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
        return Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, IBinarizationOptions options)
    {
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();
        return Result.Ok();
    }
}

