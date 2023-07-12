namespace BisUtils.P3D.Models.Data;

using Core.Binarize;
using Core.Binarize.Implementation;
using BisUtils.Core.Binarize.Options;
using Core.IO;
using Face;
using Options;
using FResults;
using Utils;

public interface IRVShapeData : IStrictBinaryObject<RVShapeOptions>
{
    IEnumerable<IRVVector> Normals { get; }
    IEnumerable<IRVVector> Points { get; }
    IEnumerable<IRVFace> Faces { get; }
}

public class RVShapeData : StrictBinaryObject<RVShapeOptions>, IRVShapeData
{
    protected const uint ValidVersion = 256;
    public IEnumerable<IRVVector> Points => MutablePoints;
    public IEnumerable<IRVVector> Normals => MutableNormals;
    public IEnumerable<IRVFace> Faces => MutableFaces;

    public List<IRVVector> MutablePoints { get; set; } = null!;
    public List<IRVVector> MutableNormals { get; set; } = null!;
    public List<IRVFace> MutableFaces { get; set; } = null!;


    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) => throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        var magic = reader.ReadAscii(4, options);
        var headSize = reader.ReadInt32();
        options.FaceVersion = reader.ReadInt32();
        var pointCount = reader.ReadInt32();
        var normalCount = reader.ReadInt32();
        var facesCount = reader.ReadInt32();
        var flags = reader.ReadInt32();
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

        MutablePoints = reader
            .ReadIndexedList<RVVector, IBinarizationOptions>(pointCount, options)
            .Cast<IRVVector>()
            .ToList();
        MutableNormals = reader
            .ReadIndexedList<RVVector, IBinarizationOptions>(normalCount, options)
            .Cast<IRVVector>()
            .ToList();
        MutableFaces = reader
            .ReadStrictIndexedList<RVFace, RVShapeOptions>(facesCount, options)
            .Cast<IRVFace>()
            .ToList();
        return Result.Ok();
    }
    public override Result Validate(RVShapeOptions options) => throw new NotImplementedException();
}
