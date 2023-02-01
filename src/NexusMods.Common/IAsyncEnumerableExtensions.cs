namespace NexusMods.Common;

public static class IAsyncEnumerableExtensions
{
    /// <summary>
    /// Filter a IAsyncEnumerable<T> by a given predicate
    /// </summary>
    /// <param name="coll"></param>
    /// <param name="func"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async IAsyncEnumerable<T> Where<T>(this IAsyncEnumerable<T> coll, Func<T, bool> func)
    {
        await foreach (var itm in coll)
            if (func(itm))
                yield return itm;
    }

    public static async IAsyncEnumerable<TOut> SelectMany<TIn, TOut>(this IEnumerable<TIn> coll,
        Func<TIn, IAsyncEnumerable<TOut>> f, CancellationToken token = default)
    {
        foreach (var itm in coll)
            await foreach (var innerItm in f(itm))
                yield return innerItm;
    }
}