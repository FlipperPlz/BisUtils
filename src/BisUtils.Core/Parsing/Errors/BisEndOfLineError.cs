namespace BisUtils.Core.Parsing.Errors;

using FResults.Reasoning;
using Lexer;

/// <summary>
/// Represents an error that is thrown when the end of line (EOL) is reached prematurely while parsing a binary string.
/// This class is a singleton, and should be accessed via the <see cref="Instance"/> property.
/// </summary>
public class BisEndOfLineError : ErrorBase
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="BisEndOfLineError"/> class.
    /// </summary>
    public static readonly BisEndOfLineError Instance = new();

    /// <summary>
    /// Gets the name of the alert associated with this error.
    /// </summary>
    public override string? AlertName
    {
        get => "ReachedEOL";
        init => throw new NotSupportedException();
    }

    /// <summary>
    /// Gets the scope of the alert associated with this error.
    /// </summary>
    public override Type? AlertScope
    {
        get => null;
        init => throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the message associated with this error.
    /// </summary>
    public override string? Message
    {
        get => "The EOL was reached prematurely.";
        set => throw new NotSupportedException();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BisEndOfLineError"/> class with the specified metadata.
    /// </summary>
    /// <param name="metadata">The metadata to associate with the error.</param>
    /// <returns>A <see cref="BisEndOfLineError"/> instance with the specified metadata.</returns>
    public static BisEndOfLineError CreateInstanceWithData(Dictionary<string, object> metadata) =>
        new() { Metadata = metadata };
}
