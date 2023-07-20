namespace BisUtils.Core.Binarize.Validatable;

using Core.Options;
using FResults;

/// <summary>
/// Defines an interface for objects that can undergo some form of validation as per specific options.
/// </summary>
/// <typeparam name="TOptions">The type of the options object that controls the validation process.</typeparam>
public interface IValidatable<in TOptions> where TOptions : IBisOptions
{
    /// <summary>
    /// Validates the current object according to the provided options.
    /// </summary>
    /// <param name="options">A `T` instance that prescribes the parameters of the function.</param>
    /// <returns>A <see cref="Result"/> object indicating the outcome of the operation.</returns>
    public Result Validate(TOptions options);
}
