namespace BisUtils.Core.Parsing;

using System.Text.RegularExpressions;

/// <summary>
/// Interface representing a mutable string stepper.
/// Derives from IBisStringStepper.
/// </summary>
public interface IBisMutableStringStepper : IBisStringStepper
{
    /// <summary>
    /// Replaces the range of the string with the specified replacement.
    /// </summary>
    void ReplaceRange(Range range, string replacement);

    /// <summary>
    /// Removes the range of the string and outputs the removed text.
    /// </summary>
    void RemoveRange(Range range, out string removedText);

    /// <summary>
    /// Replaces the substring from the current position till the specified index.
    /// </summary>
    void ReplaceUntil(int until, string pattern, string replaceWith);

    /// <summary>
    /// Returns true if the string region from the current position matches the specified text.
    /// </summary>
    bool RegionMatches(string text);

    /// <summary>
    /// Replaces all occurrences of a substring in the string.
    /// </summary>
    void ReplaceAll(string pattern, string replaceWith);

    /// <summary>
    /// Replaces all pattern matches defined by the regex in the string.
    /// </summary>
    void ReplaceAll(Regex pattern, string replaceWith);
}

public class BisMutableStringStepper : BisStringStepper, IBisMutableStringStepper
{
    public BisMutableStringStepper(string content) : base(content)
    {
    }

    /// <inheritdoc />
    public void ReplaceRange(Range range, string replacement)
    {
        int start = range.Start.Value, end = range.End.Value;

        if (start < 0 || start > Content.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(range), "Starting index is out of bounds.");
        }

        if (end < start || end > Content.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(range), "Ending index is out of bounds.");
        }

        Content = string.Concat(Content.AsSpan(0, start), replacement, Content.AsSpan(end));
    }

    /// <inheritdoc />
    public void RemoveRange(Range range, out string removedText)
    {
        int start = range.Start.Value, end = range.End.Value;

        if (start < 0 || start > Content.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(range), "Starting index is out of bounds.");
        }

        if (end < start || end > Content.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(range), "Ending index is out of bounds.");
        }
        removedText = Content[start..end];
        Content = Content.Remove(start, end - start);
    }

    /// <inheritdoc />
    public void ReplaceUntil(int until, string pattern, string replaceWith)
    {
        var substring = Content.Substring(Position, until - Position);
        var replacedSubstring = substring.Replace(pattern, replaceWith);
        Content = Content.Remove(Position, until - Position).Insert(Position, replacedSubstring);
    }

#pragma warning disable CA1310 //TODO: Localize
    /// <inheritdoc />
    public bool RegionMatches(string text) => Content[Position..].StartsWith(text);
#pragma warning restore CA1310

    /// <inheritdoc />
    public void ReplaceAll(string pattern, string replaceWith) => Content = Content.Replace(pattern, replaceWith);

    /// <inheritdoc />
    public void ReplaceAll(Regex pattern, string replaceWith) => Content = pattern.Replace(Content, replaceWith);
}
