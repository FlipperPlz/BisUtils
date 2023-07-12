namespace BisUtils.P3D.Models.Data;

using Core.Binarize;
using Core.Binarize.Implementation;
using BisUtils.Core.Binarize.Options;
using Core.Extensions;
using Core.IO;
using Face;
using Options;
using FResults;
using Point;
using Utils;

public interface IRVShapeData : IStrictBinaryObject<RVShapeOptions>
{
    public IRVResolution Resolution { get; set; }
    List<IRVVector> Normals { get; set; }
    List<IRVVector> Points { get; set; }
    List<IRVFace> Faces { get; set; }
    IRVPointAttrib<float> Mass { get; set; }
}

public class RVShapeData : StrictBinaryObject<RVShapeOptions>, IRVShapeData
{

    protected const uint ValidVersion = 256;
    public IRVResolution Resolution { get; set; } = null!;
    public List<IRVVector> Points { get; set; } = null!;
    public List<IRVVector> Normals { get; set; } = null!;
    public List<IRVFace> Faces { get; set; } = null!;
    public IRVPointAttrib<float> Mass { get; set; } = null!;

    public RVShapeData()
    {
    }

    public RVShapeData(BisBinaryReader reader, RVShapeOptions options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RVShapeData(IRVResolution resolution, List<IRVVector> points, List<IRVVector> normals, List<IRVFace> faces, IRVPointAttrib<float> mass)
    {
        Resolution = resolution;
        Points = points;
        Normals = normals;
        Faces = faces;
        Mass = mass;
    }

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) => throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        var magic = reader.ReadAscii(4, options);
        var headSize = reader.ReadInt32();
        options.FaceVersion = reader.ReadInt32();
        var pointCount = reader.ReadInt32();
        var normalCount = reader.ReadInt32();
        var facesCount = reader.ReadInt32();
        /*TODO: var flags = */reader.ReadInt32();
        options.ExtendedFace = false;

        switch (magic)
        {
            case "P3DM":
            {
                options.ExtendedFace = true;
                if (options.FaceVersion != ValidVersion)
                {
                    //return Result.Warn()
                    throw new NotImplementedException();
                }

                options.FaceVersion = 1;

                reader.BaseStream.Seek(headSize - (24 + magic.Length), SeekOrigin.Current);
                break;
            }
            case "SP3X":
            {
                options.FaceVersion = 0;
                options.ExtendedFace = true;
                reader.BaseStream.Seek(headSize - (24 + magic.Length), SeekOrigin.Current);
                break;
            }
            case "SP3D":
            {
                pointCount = headSize;
                normalCount = options.FaceVersion;
                facesCount = pointCount;

                // ReSharper disable once RedundantAssignment
                headSize = 12 + magic.Length;
                options.FaceVersion = 0;
                break;
            }
            default:
            {
                //return Result.Warn($"Bad file format {magic}");
                throw new NotImplementedException();
            }
        }

        Points = reader
            .ReadIndexedList<RVVector, IBinarizationOptions>(options, pointCount)
            .Cast<IRVVector>()
            .ToList();
        Normals = reader
            .ReadIndexedList<RVVector, IBinarizationOptions>(options, normalCount).Cast<IRVVector>()
            .ToList();
        Faces = reader
            .ReadStrictIndexedList<RVFace, RVShapeOptions>(options, facesCount)
            .Cast<IRVFace>()
            .ToList();
        Resolution = (RVResolution) reader.ReadSingle();

        return Result.Ok();
    }

    public override Result Validate(RVShapeOptions options) => throw new NotImplementedException();
}
