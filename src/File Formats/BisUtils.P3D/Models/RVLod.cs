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
    protected const uint Signature1 = 28, Signature2 = 256;
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
        if (reader.ReadAscii(4, options) != "P3DM")
        {
            return Result.Fail(new LodReadError("Currently only P3DM LODs are supported.", "Invalid Level Of Detail Signature"));
        }

        if (reader.ReadUInt32() != Signature1 || reader.ReadUInt32() != Signature2)
        {
            return Result.Fail(new LodReadError("Unknown P3DMLOD Version", "Invalid Level Of Detail Signature"));
        }


        return Result.Ok();
    }
    public override Result Validate(RVShapeOptions options) => throw new NotImplementedException();
}
