namespace BisUtils.Core.Binarize;

using FResults;
using IO;
using Options;

public interface IDebinarizable<in TDebinarizationOptions> where TDebinarizationOptions : IBinarizationOptions
{
    public Result Debinarize(BisBinaryReader reader, TDebinarizationOptions options);
}
