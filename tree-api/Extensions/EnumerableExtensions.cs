namespace tree_api.Extensions;

internal static class EnumerableExtensions
{
    public static TResult[] ToArray<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, TResult> selector)
    {
        return source.Select(selector).ToArray();
    }
}
