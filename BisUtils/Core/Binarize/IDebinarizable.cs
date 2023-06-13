using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.IO;

namespace BisUtils.Core.Binarize;

public interface IDebinarizable<in TDebinarizationOptions> where TDebinarizationOptions : IBinarizationOptions
{
    public BinarizationResult Debinarize(BisBinaryReader reader, TDebinarizationOptions options);
}