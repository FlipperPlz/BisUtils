namespace BisUtils.Core.Binarize;

using FResults;

/// <summary>
/// Defines an object which can store the result of a function execution to be retrieved later.
/// This could be useful where the result of a function is expensive to compute and is needed multiple times.
/// </summary>
public interface ICachedResult
{
    /// <summary>
    /// Gets the last result computed by a function execution.
    /// </summary>
    public Result? LastResult { get; }
}
