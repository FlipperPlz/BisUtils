namespace BisUtils.Core.ParsingFramework.Steppers.Mutable;

using System.Text.RegularExpressions;
using Immutable;

public interface IBisMutableStringStepper : IBisStringStepper
{
    /// <summary>
    /// Replaces the range of the string with the specified replacement.
    /// </summary>
    void ReplaceRange(Range range, string replacement, out string replacedText);

    /// <summary>
    /// Replaces all pattern matches defined by the regex in the string.
    /// </summary>
    void ReplaceAll(Regex pattern, string replaceWith);
}
