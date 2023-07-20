namespace BisUtils.Core.Extensions;

using System.Diagnostics.CodeAnalysis;

public static class ResultsExtensions
{
    [DoesNotReturn]
    public static void Throw(this FResults.Result result) => throw new NotSupportedException(result.ToString());
}
