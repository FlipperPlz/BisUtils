namespace BisUtils.Core.Binarize;

using Options;

/// <summary>
/// Defines a binary object which gives strict control over the binarization and debinarization process by enabling validation,
/// using the same provided options, before executing binarization.
/// </summary>
/// <typeparam name="T">The type of the binarization and debinarization options to be used with the methods.</typeparam>
public interface IStrictBinaryObject<in T> : IStrictBinarizable<T>, IDebinarizable<T> where T : IBinarizationOptions
{
}
