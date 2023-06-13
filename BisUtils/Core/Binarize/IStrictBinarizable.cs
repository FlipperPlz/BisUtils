using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Binarize.Validatable;

namespace BisUtils.Core.Binarize;

public interface IStrictBinarizable<in TBinarizationOptions> : 
    IBinarizable<TBinarizationOptions>,
    IValidatable<TBinarizationOptions> where TBinarizationOptions : IBinarizationOptions {}