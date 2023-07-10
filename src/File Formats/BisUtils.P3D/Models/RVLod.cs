namespace BisUtils.P3D.Models;

using System.Numerics;
using Core.Binarize;
using Core.Binarize.Implementation;
using Core.IO;
using Errors;
using FResults;
using Options;

public interface IRVLod : IStrictBinaryObject<RVShapeOptions>
{
    string Name { get; }
    IRVResolution Resolution { get; set; }
    IEnumerable<Vector3> Points { get; }
    IEnumerable<Vector3> Normals { get; }
    IEnumerable<string> Textures { get; }
    IEnumerable<string> Materials { get; }
}

public class RVLod : StrictBinaryObject<RVShapeOptions>, IRVLod
{
    protected const uint ValidVersion = 256;
    public string Name => Resolution.Name;
    public IRVResolution Resolution { get; set; }
    public IEnumerable<Vector3> Points => points;
    public virtual IEnumerable<Vector3> Normals => normals;
    public virtual IEnumerable<string> Textures => textures;
    public virtual IEnumerable<string> Materials => materials;

    protected List<Vector3> points { get; set; } = new();
    protected List<Vector3> normals { get; set; } = new();
    protected List<string> textures { get; set; } = new();
    protected List<string> materials { get; set; } = new();

    public RVLod(IRVResolution resolution) => Resolution = resolution;
    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) => throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        var headStart = reader.BaseStream.Position;
        var magic = reader.ReadAscii(4, options);
        var headSize = reader.ReadUInt32();
        var version = reader.ReadUInt32();
        var pointCount = reader.ReadUInt32();
        var normalCount = reader.ReadUInt32();
        var facesCount = reader.ReadUInt32();
        var flags = reader.ReadUInt32();
        var extended = false;
        var material = false;

        switch (magic)
        {
            case "P3DM":
            {
                material = true;
                extended = true;
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
                extended = true;
                reader.BaseStream.Seek(headSize - (24 + magic.Length), SeekOrigin.Current);
                break;
            }
            case "SP3D":
            {
                version = 0;

                pointCount = headSize;
                normalCount = version;
                facesCount = pointCount;
                headSize = (uint)(12 + magic.Length);
                break;
            }
            default:
            {
                //return Result.Warn($"Bad file format {magic}");
                throw new NotImplementedException();
            }
        }


        return Result.Ok();
    }
    public override Result Validate(RVShapeOptions options) => throw new NotImplementedException();
}
