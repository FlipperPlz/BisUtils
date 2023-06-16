namespace BisUtils.Core.Binarize;

using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Binarize.Validatable;

public interface IStrictBinarizable<in TBinarizationOptions> :
    IBinarizable<TBinarizationOptions>,
    IValidatable<TBinarizationOptions> where TBinarizationOptions : IBinarizationOptions {}
