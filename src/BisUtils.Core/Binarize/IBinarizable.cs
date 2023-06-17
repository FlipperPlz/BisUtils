namespace BisUtils.Core.Binarize;

using FResults;
using IO;
using Options;
using Utils;

public interface IBinarizable<in TBinarizationOptions> : ICachedResult where TBinarizationOptions : IBinarizationOptions
{
    [MustBeValidated("Object is not currently in a valid state to be written.")]
    public Result Binarize(BisBinaryWriter writer, TBinarizationOptions options);
}
