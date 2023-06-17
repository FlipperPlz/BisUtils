namespace BisUtils.Core.Binarize;

using BisUtils.Core.Binarize.Options;

public interface IBinaryObject<in T> : IBinarizable<T>, IDebinarizable<T> where T : IBinarizationOptions
{
}
