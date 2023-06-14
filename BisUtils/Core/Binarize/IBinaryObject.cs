using BisUtils.Core.Binarize.Options;

namespace BisUtils.Core.Binarize;

public interface IBinaryObject<in T> : IBinarizable<T>, IDebinarizable<T> where T : IBinarizationOptions
{
}
