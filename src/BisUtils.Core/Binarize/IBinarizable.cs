namespace BisUtils.Core.Binarize;

using FResults;
using IO;
using Options;
using Utils;

/// <summary>
/// Defines functionality for objects in the framework that can be binarized using specific options.
/// Implement this interface when objects need to provide custom logic for binary formatting.
/// </summary>
/// <typeparam name="TBinarizationOptions">Specifies the type of the binarization options to be used by the <see cref="Binarize"/> method.</typeparam>
public interface IBinarizable<in TBinarizationOptions> : ICachedResult where TBinarizationOptions : IBinarizationOptions
{
    /// <summary>
    /// Binarizes the current object according to the provided options into the specified <see cref="BisBinaryWriter"/>.
    /// If the current object's state is not valid for binarization, this method should return a failed Result.
    /// </summary>
    /// <param name="writer">The binary writer used to write the binarized data.</param>
    /// <param name="options">The options controlling the binarization process.</param>
    /// <returns>A <see cref="Result"/> object indicating the operation's success or failure.</returns>
    [MustBeValidated("Object is not currently in a valid state to be written.")]
    public Result Binarize(BisBinaryWriter writer, TBinarizationOptions options);
}
