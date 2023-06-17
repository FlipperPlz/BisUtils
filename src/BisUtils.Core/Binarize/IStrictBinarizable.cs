namespace BisUtils.Core.Binarize;

using Options;
using Validatable;

public interface IStrictBinarizable<in TBinarizationOptions> :
    IBinarizable<TBinarizationOptions>,
    IValidatable<TBinarizationOptions> where TBinarizationOptions : IBinarizationOptions
{
}
