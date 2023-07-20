namespace BisUtils.Core.Binarize.Exceptions;

/// <summary>
/// The exception that is thrown when a debinarization operation fails.
/// This class inherits from the IOException class and can thus be caught as general I/O exceptions.
/// </summary>
public class DebinarizeFailedException : IOException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DebinarizeFailedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DebinarizeFailedException(string? message) : base(message)
    {
    }
}
