namespace BisUtils.Core.Parsing;

using System.Text;
using Error;
using Extensions;

/// <summary>
/// Provides the functions required for stepping through a string.
/// </summary>
public interface IBisStringStepper
{
    /// <summary>
    /// Gets the content currently being processed.
    /// </summary>
    protected string Content { get; }

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
    /// Checks if the string stepper has a next character in its content.
    /// </summary>
    /// <returns>A boolean value indicating if the next character exists.</returns>
    public bool HasNext();

    /// <summary>
    /// Checks if the string stepper has reached the end of its content.
    /// </summary>
    /// <returns>A boolean value indicating if the end of the file has been reached.</returns>
    public bool IsEOF();

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
    /// Peeks forward from the current position without moving the actual position.
    /// </summary>
    /// <param name="count">Number of positions to peek forward.</param>
    /// <returns>The character at the peeked position.</returns>
    public char? PeekForward(int count = 1);

    /// <summary>
    /// Peeks backward from the current position without moving the actual position.
    /// </summary>
    /// <param name="count">Number of positions to peek backward.</param>
    /// <returns>The character at the peeked position.</returns>
    public char? PeekBackward(int count = 1);

    /// <summary>
    /// Moves the position to a specific index in the content.
    /// </summary>
    /// <param name="position">The index to jump to in the content.</param>
    /// <returns>The character at the new position.</returns>
    public char? JumpTo(int position);

    /// <summary>
    /// Looks at the character at a specified position without changing the string stepper's position.
    /// </summary>
    /// <param name="position">The position to peek at.</param>
    /// <returns>The character at the peeked position.</returns>
    public char? PeekAt(int position);

    /// <summary>
    /// Resets the internal state of the string stepper.
    /// </summary>
    /// <param name="content">The content to reset to. If null, resets position and related state only.</param>
    public void ResetLexer(string? content = null);

    /// <summary>
    /// Reads and moves the position forward by a certain count.
    /// </summary>
    /// <param name="count">The number of characters to read.</param>
    /// <param name="includeFirst">Should the first character be included in the count.</param>
    /// <returns>The string read.</returns>
    public string ReadChars(int count, bool includeFirst = false);

    /// <summary>
    /// Gets part of the string from the specified range.
    /// </summary>
    /// <param name="range">The range in the content to retrieve.</param>
    /// <returns>The string at the specified range.</returns>
    public string GetRange(Range range);

    /// <summary>
    /// Peeks at multiple characters forward from the current position without moving the actual position.
    /// </summary>
    /// <param name="count">Number of characters to peek forward.</param>
    /// <returns>The string at the peeked range.</returns>
    public string PeekForwardMulti(int count = 1);

    /// <summary>
    /// Peeks at multiple characters backward from the current position without moving the actual position.
    /// </summary>
    /// <param name="count">Number of characters to peek backward.</param>
    /// <returns>The string at the peeked range.</returns>
    public string PeekBackwardMulti(int count = 1);

    public int Length => Content.Length;


    /// <summary>
    /// Scans content forward until specified condition is met.
    /// </summary>
    /// <param name="until">The condition that stops the scanning.</param>
    /// <param name="consumeCurrent">Includes the current character in the scanning.</param>
    /// <returns>The scanned string.</returns>
    public string ScanUntil(Func<char, bool> until, bool consumeCurrent = false);
}

public class BisStringStepper : IBisStringStepper
{
    /// <inheritdoc />
    public string Content { get; protected set; }

    protected BisStringStepper(string content) => Content = content;


    /// <inheritdoc />
    public int Position { get; private set; } = -1;

    /// <inheritdoc />
    public char? CurrentChar { get; private set; }

    /// <inheritdoc />
    public char? PreviousChar { get; private set; }

    /// <inheritdoc />
    public bool HasNext() => PeekForward() is not null;

    /// <inheritdoc />
    public bool IsEOF() => CurrentChar is null && Position > Content.Length;

    /// <inheritdoc />
    public char? MoveForward(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        return JumpTo(Position + count);
    }

    /// <inheritdoc />
    public char? MoveBackward(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        return JumpTo(Position - count);
    }

    /// <inheritdoc />
    public char? PeekForward(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        return Content.GetOrNull(Position + count);
    }


    /// <inheritdoc />
    public string ReadChars(int count, bool includeFirst = false)
    {
        var i = 0;
        var builder = new StringBuilder();
        if (includeFirst && count >= 1)
        {
            i++;
            builder.Append(CurrentChar);
        }

        while (i != count)
        {
            i++;
            builder.Append(MoveForward());
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public string GetRange(Range range) => Content[range];

    public string GetWhile(Func<BisStringStepper, bool> condition)
    {
        var builder = new StringBuilder();
        while (condition(this))
        {
            builder.Append(CurrentChar);
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public char? PeekAt(int position) => Content.GetOrNull(position);

    /// <inheritdoc />
    public void ResetLexer(string? content = null)
    {
        if (content is not null)
        {
            Content = content;
        }

        Position = -1;
        PreviousChar = null;
        CurrentChar = null;
    }

    /// <inheritdoc />
    public string PeekForwardMulti(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        var endPosition = Position + count;

        if (endPosition > Content.Length)
        {
            endPosition = Content.Length;
        }

        return Content.Substring(Position, endPosition - Position);
    }

    /// <inheritdoc />
    public char? PeekBackward(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        return Content.GetOrNull(Position - count);
    }

    /// <inheritdoc />
    public string PeekBackwardMulti(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        var startPosition = Position - count;

        if (startPosition < 0)
        {
            startPosition = 0;
        }

        return Content.Substring(startPosition, Position - startPosition);
    }

    /// <inheritdoc />
    public string ScanUntil(Func<char, bool> until, bool consumeCurrent = false)
    {
        var builder = new StringBuilder();
        if (consumeCurrent)
        {
            builder.Append(CurrentChar);
        }

        while (PeekForward() is { } current && !until(current))
        {
            builder.Append(MoveForward());
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public char? JumpTo(int position)
    {
        Position = position;
        PreviousChar = Content.GetOrNull(position - 1);
        return CurrentChar = Content.GetOrNull(position);
    }

    public override string ToString() => Content;
}
