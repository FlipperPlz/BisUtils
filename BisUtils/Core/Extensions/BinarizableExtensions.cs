using BisUtils.Core.Binarize;
using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.Binarize.Validatable;
using BisUtils.Core.IO;

namespace BisUtils.Core.Extensions;

public static class BinarizableExtensions
{
    public static BinarizationResult BinarizeValidated<TBinarizationOptions>(
        this IStrictBinarizable<TBinarizationOptions> binarizable,
        BisBinaryWriter writer,
        TBinarizationOptions options
    ) where TBinarizationOptions : IBinarizationOptions
    {
        var validatable = (IValidatable<TBinarizationOptions>) binarizable;
        if (options.IgnoreValidation || validatable.Validate(options))
        {
            return binarizable.Binarize(writer, options);
        }

        if (
            binarizable.GetType()
                .GetMethod(nameof(IBinarizable<TBinarizationOptions>.Binarize))?
                .GetCustomAttributes(typeof(MustBeValidatedAttribute), true)
                .FirstOrDefault() is MustBeValidatedAttribute attribute
            )
        {
            return new BinarizationResult(attribute.ErrorMessage); //TODO: Validation should return its own result
        }
        return binarizable.Binarize(writer, options);

    }
}
