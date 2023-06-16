namespace BisUtils.Core.Extensions;

using Binarize;
using BisUtils.Core.Binarize.Options;
using Binarize.Utils;
using Binarize.Validatable;
using IO;
using FResults;

public static class BinarizableExtensions
{
    public static Result BinarizeValidated<TBinarizationOptions>(
        this IStrictBinarizable<TBinarizationOptions> binarizable,
        BisBinaryWriter writer,
        TBinarizationOptions options
    ) where TBinarizationOptions : IBinarizationOptions
    {
        var validatable = (IValidatable<TBinarizationOptions>) binarizable;
        if (options.IgnoreValidation || validatable.Validate(options).IsSuccess)
        {
            return binarizable.Binarize(writer, options);
        }

        if (binarizable.GetType()
                .GetMethod(nameof(IBinarizable<TBinarizationOptions>.Binarize))?
                .GetCustomAttributes(typeof(MustBeValidatedAttribute), true)
                .FirstOrDefault() is not MustBeValidatedAttribute )
        {
            return binarizable.Binarize(writer, options);
        }

        var result = binarizable.Validate(options); //TODO: Validation result;; merge
        return result.IsFailed ? result : binarizable.Binarize(writer, options).WithWarnings(result.Warnings);
    }
}
