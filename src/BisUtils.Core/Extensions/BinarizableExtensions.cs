namespace BisUtils.Core.Extensions;

using System.Text;
using Binarize;
using Binarize.Options;
using Binarize.Utils;
using FResults;
using IO;
using Parsing;
using Parsing.Lexer;

/// <summary>
/// Provides extension methods for objects that implement <see cref="IStrictBinarizable{TBinarizationOptions}"/>
/// </summary>
public static class BinarizableExtensions
{

    /// <summary>
    /// Performs binary formatting of the given <see cref="IStrictBinarizable{TBinarizationOptions}"/> object.
    /// If the options indicate to not ignore validation and the validation is successful, the binarize method is called.
    /// If the binarize method is decorated with the <see cref="MustBeValidatedAttribute"/> attribute, validation is performed
    /// before the binarization process, regardless of the `IgnoreValidation` property.
    /// </summary>
    /// <param name="binarizable">The <see cref="IStrictBinarizable{TBinarizationOptions}"/> object to be binarized.</param>
    /// <param name="writer">The binary writer to write the binarized data to.</param>
    /// <param name="options">The binarization options.</param>
    /// <typeparam name="TBinarizationOptions">The type of binarization options.</typeparam>
    /// <returns>Returns a <see cref="Result"/> object that indicates success or failure.</returns>
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
