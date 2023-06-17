using System.Text.RegularExpressions;

namespace BisUtils.Core.Parsing;

public class BisMutableStringStepper : BisStringStepper
{
    public BisMutableStringStepper(string content) : base(content)
    {
    }

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

    public void ReplaceUntil(int until, string from, string to)
    {
        var substring = Content.Substring(Position, until - Position);
        var replacedSubstring = substring.Replace(from, to);
        Content = Content.Remove(Position, until - Position).Insert(Position, replacedSubstring);
    }

    //TODO: Localize
    public bool RegionMatches(string text) => Content[Position..].StartsWith(text);

    public void ReplaceAll(string from, string to) => Content = Content.Replace(from, to);

    public void ReplaceAll(Regex from, string to) => Content = from.Replace(Content, to);


}
