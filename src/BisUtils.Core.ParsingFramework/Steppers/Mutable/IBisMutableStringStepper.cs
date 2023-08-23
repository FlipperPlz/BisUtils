namespace BisUtils.Core.ParsingFramework.Steppers.Mutable;

using System.Text.RegularExpressions;
using Immutable;

public interface IBisMutableStringStepper : IBisStringStepper
{
    /// <summary>
    /// Enumeration for text replacement position options.
    /// </summary>
    public enum TextReplacementPositionOption : byte
    {
        /// <summary>
        /// HugRight indicates the position should be set to the upper bound.
        /// </summary>
        HugRight,

        /// <summary>
        /// HugLeft indicates the position should be set to the lower bound.
        /// </summary>
        HugLeft,

        /// <summary>
        /// KeepRemaining indicates the position should be the total length minus the remaining value.
        /// </summary>
        KeepRemaining,

        /// <summary>
        /// Reset indicates the position should be reset.
        /// </summary>
        Reset,

        /// <summary>
        /// DontTouch indicates the position should be kept as it is.
        /// </summary>
        DontTouch
    }



    /// <summary>
    /// Replaces the range of the string with the specified replacement.
    /// </summary>
    void ReplaceRange(Range range, string replacement, out string replacedText, TextReplacementPositionOption endPositionOption);

    /// <summary>
    /// Replaces all pattern matches defined by the regex in the string.
    /// </summary>
    void ReplaceAll(Regex pattern, string replaceWith, TextReplacementPositionOption endPositionOption);


}
