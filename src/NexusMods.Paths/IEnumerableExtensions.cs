namespace NexusMods.Paths;

public static class IEnumerableExtensions
{
    public static Size Sum<T>(this IEnumerable<T> coll, Func<T, Size> f)
    {
        return coll.Aggregate(Size.Zero, (acc, i) => acc + f(i));
    }
}