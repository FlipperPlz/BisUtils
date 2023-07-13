namespace BisUtils.P3D.Models.Lod;

using System.Text;
using Core.Binarize;
using Core.Binarize.Implementation;
using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Extensions;
using Core.IO;
using Errors;
using Face;
using Point;
using Utils;
using Options;
using FResults;
using FResults.Extensions;

public interface IRVLod : IStrictBinaryObject<RVShapeOptions>
{
    public IRVResolution Resolution { get; set; }
    List<IRVVector> Normals { get; set; }
    List<IRVPoint> Points { get; set; }
    List<IRVFace> Faces { get; set; }
    List<IRVDataVertex> Vertices { get; set; }
    IRVPointAttrib<float> Mass { get; set; }
}

public class RVLod : StrictBinaryObject<RVShapeOptions>, IRVLod
{

    protected const uint ValidVersion = 256;
    public IRVResolution Resolution { get; set; } = null!;
    public List<IRVPoint> Points { get; set; } = null!;
    public List<IRVVector> Normals { get; set; } = null!;
    public List<IRVFace> Faces { get; set; } = null!;
    public IRVPointAttrib<float> Mass { get; set; } = null!;
    public List<IRVDataVertex> Vertices { get; set; } = null!;

    public RVLod()
    {
    }

    public RVLod(BisBinaryReader reader, RVShapeOptions options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RVLod(IRVResolution resolution, List<IRVDataVertex> vertices, List<IRVPoint> points, List<IRVVector> normals, List<IRVFace> faces, IRVPointAttrib<float> mass)
    {
        Vertices = vertices;
        Resolution = resolution;
        Points = points;
        Normals = normals;
        Faces = faces;
        Mass = mass;
    }

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) => throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        LastResult = Result.Ok();

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
                options.ExtendedPoint = true;
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
                options.ExtendedPoint = true;
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
            .ReadIndexedList<RVPoint, RVShapeOptions>(options, pointCount)
            .Cast<IRVPoint>()
            .ToList();
        Normals = reader
            .ReadIndexedList<RVVector, IBinarizationOptions>(options, normalCount).Cast<IRVVector>()
            .ToList();
        Faces = reader
            .ReadStrictIndexedList<RVFace, RVShapeOptions>(options, facesCount)
            .Cast<IRVFace>()
            .ToList();
        Resolution = (RVResolution) reader.ReadSingle();

        switch (reader.ReadAscii(4, options))
        {
            case "TAGG":
            {
                while (true)
                {
                    //TODO: FACE TAGGS
                    if (!LexTagg(out var taggName, reader) || taggName is null || !taggName.StartsWith('#'))
                    {
                        return LastResult.WithError(new LodReadError("Tried to parse a tag without proper prefix."));
                    }
                }
            }
            case "SS3D":
            {
                //var nVertices = reader.ReadInt32();
                //var nFaces = reader.ReadInt32();
                //var nNormals = reader.ReadInt32();
                //var normSize = reader.ReadInt32();
                //TODO(SS3D): Read
                throw new NotImplementedException();
            }
            default:
            {
                return LastResult.WithError(new LodReadError("Unknown setup magic."));
            }
        }
    }



    private static Result LexTagg(out string? taggName, BisBinaryReader reader)
    {
        var taggBuilder = new StringBuilder("#");
        var currentChar = reader.ReadChar();
        if (currentChar != '#')
        {
            taggName = null;
            return Result.Fail(new FaceReadError("Expected '#'"));
        }

        do
        {
            taggBuilder.Append(currentChar = reader.ReadChar());
        } while (currentChar != '#');

        taggName = taggBuilder.ToString();

        return Result.Ok();
    }

    public override Result Validate(RVShapeOptions options) => throw new NotImplementedException();
}
