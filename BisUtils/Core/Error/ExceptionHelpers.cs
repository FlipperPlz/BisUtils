using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace BisUtils.Core.Error;

public static class ExceptionHelpers
{

    public static void ThrowArgumentNotPositiveException([NotNull] int? number, [CallerArgumentExpression("number")] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(number, paramName);
        if (number >= 1)
        {
            return;
        }
        throw new ArgumentOutOfRangeException(paramName, "Argument must be greater than or equal to 1.");
    }

}
