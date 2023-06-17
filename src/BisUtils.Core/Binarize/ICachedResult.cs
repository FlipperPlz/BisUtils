namespace BisUtils.Core.Binarize;

using FResults;

public interface ICachedResult
{
    public Result? LastResult { get; }
}
