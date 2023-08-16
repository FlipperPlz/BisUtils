namespace BisUtils.Core.ParsingFramework.Steppers.Mutable;

using System.Text;
using System.Text.RegularExpressions;
using Immutable;
using Microsoft.Extensions.Logging;
using Misc;

public class BisMutableStringStepper : BisStringStepper, IBisMutableStringStepper
{
    public BisMutableStringStepper(string content, ILogger? logger = default) : base(content, logger)
    {
    }

    public BisMutableStringStepper(byte[] contents, Encoding encoding, ILogger? logger = default) : base(contents, encoding, logger)
    {
    }

    public BisMutableStringStepper(BinaryReader reader, Encoding encoding, int? length = null, long? stringStart = null, StepperDisposalOption readDisposalOption = StepperDisposalOption.JumpBackToStart, ILogger? logger = default) : base(reader, encoding, length, stringStart, readDisposalOption, logger)
    {
    }

    public void ReplaceRange(Range range, string replacement, out string replacedText)
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

        replacedText = GetRange(range);
        Content = string.Concat(Content.AsSpan(0, start), replacement, Content.AsSpan(end));
        JumpTo(Position);
    }

    public void ReplaceAll(Regex pattern, string replaceWith)
    {
        Content = pattern.Replace(Content, replaceWith);
        JumpTo(Position);
    }
}
