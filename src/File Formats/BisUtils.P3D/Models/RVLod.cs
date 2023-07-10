namespace BisUtils.P3D.Models;

using System.Numerics;
using Core.Binarize;
using Core.Binarize.Implementation;
using Core.IO;
using Face;
using FResults;
using Options;

public interface IRVLod : IStrictBinaryObject<RVShapeOptions>
{
    string Name { get; }
    IRVResolution Resolution { get; set; }
    IEnumerable<Vector3> Points { get; }
    IEnumerable<Vector3> Normals { get; }
    IEnumerable<IRVFace> Faces { get; }
}

public class RVLod : StrictBinaryObject<RVShapeOptions>, IRVLod
{
    protected const uint ValidVersion = 256;
    public string Name => Resolution.Name;
    public IRVResolution Resolution { get; set; }
    public virtual IEnumerable<Vector3> Points => MutablePoints;
    public virtual IEnumerable<Vector3> Normals => MutableNormals;
    public virtual IEnumerable<IRVFace> Faces => MutableFaces;

    public List<Vector3> MutablePoints { get; set; } = null!;
    public List<Vector3> MutableNormals { get; set; } = null!;
    public List<IRVFace> MutableFaces { get; set; } = null!;

    protected RVLod(IRVResolution resolution) => Resolution = resolution;

    public RVLod(BisBinaryReader reader, RVShapeOptions options) : base(reader, options)
    {
    }

    public RVLod(IRVResolution resolution, List<Vector3> mutablePoints, List<Vector3> mutableNormals, List<IRVFace> mutableFaces)
    {
        Resolution = resolution;
        MutablePoints = mutablePoints;
        MutableNormals = mutableNormals;
        MutableFaces = mutableFaces;
    }


    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) => throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        var headStart = reader.BaseStream.Position;
        var magic = reader.ReadAscii(4, options);
        var headSize = reader.ReadInt32();
        var version = reader.ReadInt32();
        var pointCount = reader.ReadInt32();
        var normalCount = reader.ReadInt32();
        var facesCount = reader.ReadInt32();
        var flags = reader.ReadInt32();

        switch (magic)
        {
            case "P3DM":
            {
                if (version != ValidVersion)
                {
                    //return Result.Warn()
                    throw new NotImplementedException();
                }

                reader.BaseStream.Seek(headSize - (24 + magic.Length), SeekOrigin.Current);
                break;
            }
            case "SP3X":
            {
                reader.BaseStream.Seek(headSize - (24 + magic.Length), SeekOrigin.Current);
                break;
            }
            case "SP3D":
            {
                version = 0;

                pointCount = headSize;
                normalCount = version;
                facesCount = pointCount;
                // ReSharper disable once RedundantAssignment
                headSize = 12 + magic.Length;
                break;
            }
            default:
            {
                //return Result.Warn($"Bad file format {magic}");
                throw new NotImplementedException();
            }
        }

        MutablePoints = new List<Vector3>(pointCount);
        MutableNormals = new List<Vector3>(normalCount);
        MutableFaces = new List<IRVFace>(facesCount);

        return Result.Ok();
    }
    public override Result Validate(RVShapeOptions options) => throw new NotImplementedException();
}
