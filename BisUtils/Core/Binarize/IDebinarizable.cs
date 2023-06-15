using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.IO;

namespace BisUtils.Core.Binarize;

using FResults;

public interface IDebinarizable<in TDebinarizationOptions> where TDebinarizationOptions : IBinarizationOptions
{
    public Result Debinarize(BisBinaryReader reader, TDebinarizationOptions options);
}
