namespace BisUtils.Core.Binarize;

using Options;

public interface IBinaryObject<in T> : IBinarizable<T>, IDebinarizable<T> where T : IBinarizationOptions
{
}
