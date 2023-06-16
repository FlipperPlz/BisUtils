namespace BisUtils.Core.Binarize;

using BisUtils.Core.Binarize.Options;

public interface IStrictBinaryObject<in T> : IStrictBinarizable<T>, IDebinarizable<T> where T : IBinarizationOptions
{

}
