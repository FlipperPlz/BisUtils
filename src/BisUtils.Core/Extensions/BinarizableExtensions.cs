namespace BisUtils.Core.Extensions;

using Binarize;
using Binarize.Options;
using Binarize.Utils;
using FResults;
using IO;

public static class BinarizableExtensions
{
    public static Result BinarizeValidated<TBinarizationOptions>(
        this IStrictBinarizable<TBinarizationOptions> binarizable,
        BisBinaryWriter writer,
        TBinarizationOptions options
    ) where TBinarizationOptions : IBinarizationOptions
    {
        if (options.IgnoreValidation || binarizable.Validate(options).IsSuccess)
        {
            return binarizable.Binarize(writer, options); //TODO: get last validation result
        }

        return binarizable.GetType()
            .GetMethod(nameof(IBinarizable<TBinarizationOptions>.Binarize))?
            .GetCustomAttributes(typeof(MustBeValidatedAttribute), true)
            .FirstOrDefault() is not MustBeValidatedAttribute
            ? binarizable.Binarize(writer, options)
            : Result.Merge(binarizable.Validate(options), binarizable.Binarize(writer, options));
    }
}
