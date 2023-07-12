namespace BisUtils.P3D.Models.Lod;

using Core.Binarize.Implementation;
using Core.IO;
using Data;
using Face;
using FResults;
using Options;
using Utils;

public interface IRVLod : IRVShapeData
{
    public string Name { get; set; }
    public IRVResolution Resolution { get; set; }
}

public class RVLod : BinaryObject<RVShapeOptions>, IRVLod
{
    public string Name { get; set; } = null!;
    public IRVResolution Resolution { get; set; } = null!;
    public IEnumerable<IRVVector> Normals { get; } = null!;
    public IEnumerable<IRVVector> Points { get; } = null!;
    public IEnumerable<IRVFace> Faces { get; } = null!;

    public RVLod()
    {
    }

    public RVLod(BisBinaryReader reader, RVShapeOptions options) : base(reader, options)
    {

    }

    public RVLod(IRVResolution resolution, IEnumerable<IRVVector> normals, IEnumerable<IRVVector> points, IEnumerable<IRVFace> faces)
    {
        Name = resolution.Name;
        Resolution = resolution;
        Normals = normals;
        Points = points;
        Faces = faces;
    }

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) => throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, RVShapeOptions options) => throw new NotImplementedException();
    public Result Validate(RVShapeOptions options) => throw new NotImplementedException();

}
