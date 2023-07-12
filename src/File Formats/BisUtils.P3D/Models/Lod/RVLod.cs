namespace BisUtils.P3D.Models.Lod;

using Core.Binarize.Implementation;
using Core.IO;
using Data;
using Errors;
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

    public override Result Debinarize(BisBinaryReader reader, RVShapeOptions options)
    {
        LastResult = Result.Ok();
        options.LodVersion = -1;
        var levelCount = options.MaxLodLevels;
        switch (reader.ReadAscii(4, options))
        {
            case "NLOD":
            {
                levelCount = reader.ReadInt32();
                options.LodVersion = 0;
                break;
            }
            case "MLOD":
            {
                options.LodVersion = reader.ReadInt32();
                if (options.LodVersion == 9999)
                {
                    options.LodVersion = 1;
                }
                break;
            }
            case "ODOL":
            {
                throw new NotImplementedException();
            }
            default: return Result.Fail(new LodReadError("Unknown lod magic, expected ODOL, MLOD, or NLOD."));
        }

        return LastResult;
    }


    public Result Validate(RVShapeOptions options) => throw new NotImplementedException();

}
