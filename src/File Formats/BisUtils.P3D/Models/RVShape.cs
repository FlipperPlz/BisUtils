namespace BisUtils.P3D.Models;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.IO;
using FResults;
using Lod;
using Options;
using Utils;

public interface IRVShape: IStrictBinaryObject<RVShapeOptions>
{
    string ModelName { get; }
    float ModelMass { get; }
    IEnumerable<IRVLod> LevelsOfDetail { get; }
    public IRVLod? this[IRVResolution resolution] =>
        LevelsOfDetail.FirstOrDefault(lod => lod.Resolution == resolution);
}

public class RVShape : StrictBinaryObject<RVShapeOptions>, IRVShape
{
    public string ModelName { get; }
    public float ModelMass { get; }
    public IEnumerable<IRVLod> LevelsOfDetail { get; } = null!;

    protected RVShape(string modelName, float modelMass, IEnumerable<IRVLod> levelsOfDetail)
    {
        ModelName = modelName;
        ModelMass = modelMass;
        LevelsOfDetail = levelsOfDetail;
    }

    protected RVShape(string modelName, BisBinaryReader reader, RVShapeOptions options) : base(reader, options) =>
        ModelName = modelName;

    public override Result Binarize(BisBinaryWriter writer, RVShapeOptions options) => throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, RVShapeOptions options) => throw new NotImplementedException();

    public override Result Validate(RVShapeOptions options) => throw new NotImplementedException();
}
