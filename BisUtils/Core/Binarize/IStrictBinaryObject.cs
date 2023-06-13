using BisUtils.Core.Binarize.Options;

namespace BisUtils.Core.Binarize;

public interface IStrictBinaryObject<in T> : IStrictBinarizable<T>, IDebinarizable<T> where T : IBinarizationOptions
{
    
}