namespace BisUtils.Core.Binarize;

using BisUtils.Core.Binarize.Options;
using BisUtils.Core.IO;
using FResults;

public interface IDebinarizable<in TDebinarizationOptions> where TDebinarizationOptions : IBinarizationOptions
{
    public Result Debinarize(BisBinaryReader reader, TDebinarizationOptions options);
}
