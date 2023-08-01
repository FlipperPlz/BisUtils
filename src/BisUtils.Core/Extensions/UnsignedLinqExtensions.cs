namespace BisUtils.Core.Extensions;

public static class UnsignedLinqExtensions
{
    public static uint UnsignedSum<TSource>(this IEnumerable<TSource> source, Func<TSource, uint> selector)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        return source.Aggregate<TSource?, uint>(0, (current, item) => current + selector(item));
    }

    public static ulong UnsignedSum<TSource>(this IEnumerable<TSource> source, Func<TSource, ulong> selector)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        return source.Aggregate<TSource?, ulong>(0, (current, item) => current + selector(item));
    }
}
