namespace BisUtils.Core.ParsingFramework.Steppers.Mutable;

using System.Text;
using System.Text.RegularExpressions;
using Extensions;
using Immutable;
using Microsoft.Extensions.Logging;
using Misc;

public class BisMutableStringStepper : BisStringStepper, IBisMutableStringStepper
{
    public BisMutableStringStepper(string content, ILogger? logger = default) : base(content, logger)
    {
    }

    public BisMutableStringStepper(BinaryReader content, Encoding encoding, StepperDisposalOption option, ILogger? logger = default, int? length = null, long? stringStart = null) :
        base(content, encoding, option, logger, length, stringStart)
    {
    }



    public void ReplaceRange(Range range, string replacement, out string replacedText, IBisMutableStringStepper.TextReplacementPositionOption endPositionOption = IBisMutableStringStepper.TextReplacementPositionOption.DontTouch)
    {
        int start = range.Start.Value, end = range.End.Value;
        var remaining = Length - Position;

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
        this.JumpToReplaceEnd(remaining, start, end, endPositionOption);
    }

    public void ReplaceAll(Regex pattern, string replaceWith, IBisMutableStringStepper.TextReplacementPositionOption endPositionOption = IBisMutableStringStepper.TextReplacementPositionOption.DontTouch)
    {
        var remaining = Length - Position;
        Content = pattern.Replace(Content, replaceWith);
        this.JumpToReplaceEnd(remaining, remaining, remaining, endPositionOption);
    }


}
