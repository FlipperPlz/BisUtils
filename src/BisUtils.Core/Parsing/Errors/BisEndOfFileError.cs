namespace BisUtils.Core.Parsing.Errors;

using FResults.Reasoning;
using Lexer;

/// <summary>
/// Represents an error that is thrown when the end of file (EOF) is reached prematurely while parsing a binary string.
/// This class is a singleton, and should be accessed via the <see cref="Instance"/> property.
/// </summary>
public class BisEndOfFileError : ErrorBase
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="BisEndOfFileError"/> class.
    /// </summary>
    public static readonly BisEndOfFileError Instance = new();

    /// <summary>
    /// Gets the name of the alert associated with this error.
    /// </summary>
    public override string? AlertName
    {
        get => "ReachedEOF";
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
        get => "The EOF was reached prematurely.";
        set => throw new NotSupportedException();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BisEndOfFileError"/> class with the specified metadata.
    /// </summary>
    /// <param name="metadata">The metadata to associate with the error.</param>
    /// <returns>A <see cref="BisEndOfFileError"/> instance with the specified metadata.</returns>
    public static BisEndOfFileError CreateInstanceWithData(Dictionary<string, object> metadata) =>
        new() { Metadata = metadata };
}
