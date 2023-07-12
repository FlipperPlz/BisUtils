namespace BisUtils.P3D.Models;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.IO;
using Lod;
using Options;

public interface IRVShape<out TLodType> : IStrictBinaryObject<RVShapeOptions> where TLodType : IRVLod
{
    string ModelName { get; }
    float ModelMass { get; }
    IEnumerable<TLodType> LevelsOfDetail { get; }
    public TLodType? this[IRVResolution resolution] =>
        LevelsOfDetail.FirstOrDefault(lod => lod.Resolution == resolution);
}

public abstract class RVShape<TLodType> : StrictBinaryObject<RVShapeOptions>, IRVShape<TLodType> where TLodType : RVLod
{
    public string ModelName { get; }
    public abstract float ModelMass { get; }
    public abstract IEnumerable<TLodType> LevelsOfDetail { get; }

    protected RVShape(string modelName) => ModelName = modelName;

    protected RVShape(string modelName, BisBinaryReader reader, RVShapeOptions options) : base(reader, options) => ModelName = modelName;

}
