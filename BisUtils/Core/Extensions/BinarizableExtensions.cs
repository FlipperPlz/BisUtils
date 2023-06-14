using BisUtils.Core.Binarize;
using BisUtils.Core.Binarize.Options;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.Binarize.Validatable;
using BisUtils.Core.IO;

namespace BisUtils.Core.Extensions;

using FluentResults;

public static class BinarizableExtensions
{
    public static Result<TBinarizationOptions> BinarizeValidated<TBinarizationOptions>(
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
            binarizable.Validate(options); //TODO: Validation result;; merge

        }
        return binarizable.Binarize(writer, options);

    }
}
