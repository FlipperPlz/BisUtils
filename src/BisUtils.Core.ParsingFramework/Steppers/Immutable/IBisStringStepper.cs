namespace BisUtils.Core.ParsingFramework.Steppers.Immutable;


/// <summary>
/// Provides an interface for a class designated to a string that can be read character by character via a sliding
/// window.
/// </summary>
public interface IBisStringStepper : IDisposable
{

    /// <summary>
    /// Gets the content currently being processed.
    /// </summary>
    protected string Content { get; }

    /// <summary>
    /// Gets the length of the buffer loaded.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets the current position in the content.
    /// </summary>
    public int Position { get; }

    /// <summary>
    /// Gets the current character in the content at the Position.
    /// </summary>
    public char? CurrentChar { get; }

    /// <summary>
    /// Gets the character in the content just before the Position.
    /// </summary>
    public char? PreviousChar { get; }

    /// <summary>
    /// Moves the position of string stepper forward by a certain count.
    /// </summary>
    /// <param name="count">Number of positions to move forward.</param>
    /// <returns>The character at the new position.</returns>
    public char? MoveForward(int count = 1);

    /// <summary>
    /// Moves the position of string stepper backward by a certain count.
    /// </summary>
    /// <param name="count">Number of positions to move backward.</param>
    /// <returns>The character at the new position.</returns>
    public char? MoveBackward(int count = 1);

    /// <summary>
    /// Looks at the character at a specified position without changing the string stepper's position.
    /// </summary>
    /// <param name="position">The position to peek at.</param>
    /// <returns>The character at the peeked position.</returns>
    public char? PeekAt(int position);

    /// <summary>
    /// Gets part of the string from the specified range.
    /// </summary>
    /// <param name="range">The range in the content to retrieve.</param>
    /// <returns>The string at the specified range.</returns>
    public string GetRange(Range range);


    /// <summary>
    /// Moves the position to a specific index in the content.
    /// </summary>
    /// <param name="position">The index to jump to in the content.</param>
    /// <returns>The character at the new position.</returns>
    public char? JumpTo(int position);

    /// <summary>
    /// Resets the internal state of the string stepper.
    /// </summary>
    /// <param name="content">The content to reset to. If null, resets position and related state only.</param>
    public void ResetStepper(string? content = null);
}
