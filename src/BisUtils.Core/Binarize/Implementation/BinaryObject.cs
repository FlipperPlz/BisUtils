namespace BisUtils.Core.Binarize.Implementation;

using FResults;
using IO;
using Options;

/// <summary>
/// Represents a binary object that can be binarized and debinarized using specified options.
/// This abstract class provides a common structure and encapsulates shared functionality for binary objects,
/// implementing the <see cref="IBinaryObject{T}"/> interface.
/// </summary>
/// <typeparam name="T">The type of the binarization options to be used by the Binarize and Debinarize methods.</typeparam>
public abstract class BinaryObject<T> : IBinaryObject<T> where T : IBinarizationOptions
{

    /// <summary>
    /// Gets the last computed binarization or debinarization result.
    /// </summary>
    public Result? LastResult { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryObject{T}"/> class, using the specified binary reader and options for debinarization.
    /// </summary>
    /// <param name="reader">The binary reader to load data from.</param>
    /// <param name="options">The options controlling the debinarization process.</param>
    protected BinaryObject(BisBinaryReader reader, T options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryObject{T}"/> class.
    /// </summary>
    protected BinaryObject()
    {
    }

    /// <summary>
    /// Binarizes the current binary object state into a binary writer as per the provided options.
    /// </summary>
    /// <param name="writer">The binary writer where the current object state will be written.</param>
    /// <param name="options">The options controlling the binarization process.</param>
    /// <returns>A Result that represents the outcome of the operation.</returns>
    public abstract Result Binarize(BisBinaryWriter writer, T options);

    /// <summary>
    /// Debinarizes data from a binary reader as per the provided options into the current object state.
    /// </summary>
    /// <param name="reader">The binary reader containing the data to be read into the current object state.</param>
    /// <param name="options">The options controlling the debinarization process.</param>
    /// <returns>A Result that represents the outcome of the operation.</returns>
    public abstract Result Debinarize(BisBinaryReader reader, T options);
}
