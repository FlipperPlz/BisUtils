namespace BisUtils.Core.Binarize;

using FResults;
using IO;
using Options;

/// <summary>
/// Defines functionality for objects in the framework that can be debinarized using specific options.
/// see <see cref="IBinarizable{TBinarizationOptions}"/> for information on the reversing process.
/// Implement this interface when objects need to provide custom logic for binary formatting reversal.
/// </summary>
/// <typeparam name="TDebinarizationOptions">Specifies the type of the debinarization options to be used by the Debinarize method.</typeparam>
public interface IDebinarizable<in TDebinarizationOptions> where TDebinarizationOptions : IBinarizationOptions
{
    /// <summary>
    /// De-binarizes data from the provided binary reader according to the provided options.
    /// </summary>
    /// <param name="reader">The binary reader containing the binarized data to parse.</param>
    /// <param name="options">The options controlling the debinarization process.</param>
    /// <returns>A `Result` object indicating the operation's success or failure.</returns>
    public Result Debinarize(BisBinaryReader reader, TDebinarizationOptions options);
}
