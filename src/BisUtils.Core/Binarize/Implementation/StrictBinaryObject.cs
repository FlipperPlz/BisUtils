namespace BisUtils.Core.Binarize.Implementation;

using FResults;
using IO;
using Options;

/// <summary>
/// Represents a strict binary object that can be validated, binarized, and debinarized using specified options.
/// This abstract class builds on the <see cref="BinaryObject{T}" /> structure, adding validation capabilities.
/// </summary>
/// <seealso cref="BinaryObject{T}" />
/// <seealso cref="IStrictBinaryObject{T}" />
/// <typeparam name="T">The type of the binarization and validation options to be used by the Binarize, Debinarize, and Validate methods.</typeparam>
public abstract class StrictBinaryObject<T> : BinaryObject<T>, IStrictBinaryObject<T> where T : IBinarizationOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictBinaryObject{T}" /> class, using the specified binary reader and options for debinarization.
    /// </summary>
    /// <param name="reader">The binary reader to load data from.</param>
    /// <param name="options">The options controlling the debinarization process.</param>
    protected StrictBinaryObject(BisBinaryReader reader, T options) : base(reader, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictBinaryObject{T}" /> class.
    /// </summary>
    protected StrictBinaryObject()
    {
    }

    /// <summary>
    /// Validates the current object using the specified options.
    /// </summary>
    /// <param name="options">Options to control the validation process.</param>
    /// <returns>A <see cref="Result"/> instance that holds the outcome of the validation process.</returns>
    public abstract Result Validate(T options);
}
