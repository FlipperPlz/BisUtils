namespace BisUtils.Core.Extensions;

public static class ListExtensions
{
    public static T? GetOrNull<T>(this IList<T>? list, int index) where T : class
    {
        if (list is not null && index >= 0 && index < list.Count) return list[index];
        return null;
    }
}