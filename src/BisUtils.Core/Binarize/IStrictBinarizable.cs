namespace BisUtils.Core.Binarize;

using Options;
using Validatable;

/// <summary>
/// Defines an framework object which can be both binarized and validated using the same options.
/// </summary>
/// <typeparam name="TBinarizationOptions">The type of the options object that controls the binarization and validation.</typeparam>
public interface IStrictBinarizable<in TBinarizationOptions> :
    IBinarizable<TBinarizationOptions>,
    IValidatable<TBinarizationOptions> where TBinarizationOptions : IBinarizationOptions
{
}
