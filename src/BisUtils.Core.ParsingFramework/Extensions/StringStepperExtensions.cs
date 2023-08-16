namespace BisUtils.Core.ParsingFramework.Extensions;

using System.Text;
using Error;
using Steppers.Immutable;
using Steppers.Mutable;

public static class StringStepperExtensions
{
    /// <summary>
    /// Peeks forward from the current position without moving the actual position.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="count">Number of positions to peek forward.</param>
    /// <returns>The character at the peeked position.</returns>
    public static char? PeekForward(this IBisStringStepper stepper, int count = 1) =>
        stepper.PeekAt(stepper.Position + count);

    /// <summary>
    /// Peeks backward from the current position without moving the actual position.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="count">Number of positions to peek backward.</param>
    /// <returns>The character at the peeked position.</returns>
    public static char? PeekBackward(this IBisStringStepper stepper, int count = 1) =>
        stepper.PeekAt(stepper.Position - count);

    /// <summary>
    /// Scans content forward until specified condition is met.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="until">The condition that stops the scanning.</param>
    /// <param name="consumeCurrent">Includes the current character in the scanning.</param>
    /// <returns>The scanned string.</returns>
    public static string ScanUntil(this IBisStringStepper stepper, Func<char, bool> until, bool consumeCurrent = false)
    {
        var builder = new StringBuilder();
        if (consumeCurrent)
        {
            builder.Append(stepper.CurrentChar);
        }

        while (PeekForward(stepper) is { } current && !until(current))
        {
            builder.Append(stepper.MoveForward());
        }

        return builder.ToString();
    }


    /// <summary>
    /// Peeks at multiple characters backward from the current position without moving the actual position.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="count">Number of characters to peek backward.</param>
    /// <returns>The string at the peeked range.</returns>
    public static string PeekBackwardMulti(this IBisStringStepper stepper, int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        int currentPosition = stepper.Position, startPosition = currentPosition - count;

        if (startPosition < 0)
        {
            startPosition = 0;
        }

        return stepper.GetRange(startPosition..currentPosition);
    }


    /// <summary>
    /// Peeks at multiple characters forward from the current position without moving the actual position.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="count">Number of characters to peek forward.</param>
    /// <returns>The string at the peeked range.</returns>
    public static string PeekForwardMulti(this IBisStringStepper stepper, int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        int currentPosition = stepper.Position, endPosition = currentPosition + count;
        if (endPosition > stepper.Length)
        {
            endPosition = stepper.Length;
        }

        return stepper.GetRange(currentPosition..endPosition);
    }

    /// <summary>
    /// Moves multiple characters forward from the current position.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="count">Number of characters to move forward.</param>
    /// <returns>The string that was scanned</returns>
    public static string MoveForwardMulti(this IBisStringStepper stepper, int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        int currentPosition = stepper.Position, endPosition = currentPosition + count;
        if (endPosition > stepper.Length)
        {
            endPosition = stepper.Length;
        }

        var str = stepper.GetRange(currentPosition..endPosition);
        stepper.JumpTo(endPosition);
        return str;
    }

    /// <summary>
    /// Moves multiple characters backward from the current position.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="count">Number of characters to move backward.</param>
    /// <returns>The string that was scanned</returns>
    public static string MoveBackwardMulti(this IBisStringStepper stepper, int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        int currentPosition = stepper.Position, endPosition = currentPosition - count;
        if (endPosition > stepper.Length)
        {
            endPosition = stepper.Length;
        }

        var str = stepper.GetRange(endPosition..currentPosition);
        stepper.JumpTo(endPosition);
        return str;
    }


    /// <summary>
    /// Reads and moves the position forward by a certain count.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="count">The number of characters to read.</param>
    /// <param name="includeFirst">Should the first character be included in the count.</param>
    /// <returns>The string read.</returns>
    public static string ReadChars(this IBisStringStepper stepper, int count, bool includeFirst = false)
    {
        var i = 0;
        var builder = new StringBuilder();
        if (includeFirst && count >= 1)
        {
            i++;
            builder.Append(stepper.CurrentChar);
        }

        while (i != count)
        {
            i++;
            builder.Append(stepper.MoveForward());
        }

        return builder.ToString();
    }

    /// <summary>
    /// Checks if the string stepper has a next character in its content.
    /// </summary>
    /// <returns>A boolean value indicating if the next character exists.</returns>
    public static bool HasNext(this IBisStringStepper stepper) => PeekForward(stepper) is not null;


    /// <summary>
    /// Checks if the string stepper has reached the end of its content.
    /// </summary>
    /// <returns>A boolean value indicating if the end of the file has been reached.</returns>
    public static bool IsEOF(this IBisStringStepper stepper) => stepper.CurrentChar is null && stepper.Position > stepper.Length;

    /// <summary>
    /// Evaluates a condition for a string stepper and returns a string containing all characters evaluated as true.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="condition">The condition applied to each character in the string stepper.</param>
    /// <returns>The string containing characters that satisfied the condition.</returns>
    public static string GetWhile(this IBisStringStepper stepper, Func<bool> condition)
    {
        var builder = new StringBuilder();
        while (condition())
        {
            builder.Append(stepper.CurrentChar);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Removes a range of characters from the string stepper.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="range">The range of positions to remove.</param>
    /// <param name="removedText">The text that was removed.</param>
    public static void RemoveRange(this IBisMutableStringStepper stepper, Range range, out string removedText) =>
        stepper.ReplaceRange(range, "", out removedText);

    /// <summary>
    /// Checks if a particular text region in the string stepper matches the specified text.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="text">The text to match.</param>
    /// <param name="comparison">The type of string comparison to perform.</param>
    /// <returns>The boolean value indicating whether the region matches the given text.</returns>
    public static bool RegionMatches(this IBisStringStepper stepper, string text, StringComparison comparison = StringComparison.CurrentCulture) =>
        stepper.MoveForwardMulti(text.Length).Equals(text, comparison);

    /// <summary>
    /// Erases the current character from the string stepper.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <returns>The character that was erased, or null if no character was removed.</returns>
    public static char? EraseCurrent(this IBisMutableStringStepper stepper) => EraseChar(stepper, stepper.Position);

    /// <summary>
    /// Erases the previous character from the string stepper.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <returns>The character that was erased, or null if no character was removed.</returns>
    public static char? ErasePrevious(this IBisMutableStringStepper stepper) => EraseChar(stepper, stepper.Position - 1);

    /// <summary>
    /// Erases a specific character from the string stepper.
    /// </summary>
    /// <param name="stepper">The string stepper</param>
    /// <param name="position">The position of the character to erase.</param>
    /// <returns>The character that was erased, or null if no character was removed.</returns>
    public static char? EraseChar(this IBisMutableStringStepper stepper, int position)
    {
        RemoveRange(stepper, position..position, out _);
        return stepper.JumpTo(stepper.Position);
    }
}
