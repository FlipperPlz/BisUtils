namespace BisUtils.Core.Binarize.Utils;

/// <summary>
/// Specifies that the attached method must be validated before it's executed.
/// The attribute carries along an error message to be used in case the method fails the validation.
/// </summary>
public class MustBeValidatedAttribute : Attribute
{
    /// <summary>
    /// Gets the error message associated with this attribute.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MustBeValidatedAttribute"/> class with a specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message to associate with a validation failure.</param>
    public MustBeValidatedAttribute(string errorMessage) => ErrorMessage = errorMessage;

}
