namespace BisUtils.Core.Extensions;

public static class StringExtensions
{
    public static char? GetOrNull(this string? str, int index)
    {
        if (str is not null && index >= 0 && index < str.Length)
        {
            return str[index];
        }

        return null;
    }
}
