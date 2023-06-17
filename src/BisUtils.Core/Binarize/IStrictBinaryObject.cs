namespace BisUtils.Core.Binarize;

using Options;

public interface IStrictBinaryObject<in T> : IStrictBinarizable<T>, IDebinarizable<T> where T : IBinarizationOptions
{
}
