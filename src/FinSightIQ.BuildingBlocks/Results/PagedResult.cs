namespace FinSightIQ.BuildingBlocks.Results;

/// <summary>
/// One page of a cursor-paginated collection, matching the FinSightIQ API <c>pagination</c> object
/// (<c>nextCursor</c>, <c>hasMore</c>, <c>limit</c>).
/// </summary>
/// <remarks>
/// The cursor is opaque to this type. Encoding, decoding and querying are the responsibility
/// of the owning service.
/// </remarks>
/// <typeparam name="T">The item type.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>The page size used when a caller supplies none.</summary>
    public const int DefaultLimit = 50;

    /// <summary>The largest permitted page size.</summary>
    public const int MaxLimit = 500;

    private PagedResult(IReadOnlyList<T> items, int limit, string? nextCursor, bool hasMore)
    {
        Items = items;
        Limit = limit;
        NextCursor = nextCursor;
        HasMore = hasMore;
    }

    /// <summary>Gets the items on this page.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>Gets the page size that was requested (not the number of items returned).</summary>
    public int Limit { get; }

    /// <summary>Gets the opaque cursor for the next page; <see langword="null"/> when <see cref="HasMore"/> is false.</summary>
    public string? NextCursor { get; }

    /// <summary>Gets a value indicating whether more items exist after this page.</summary>
    public bool HasMore { get; }

    /// <summary>Creates a page that is followed by more data.</summary>
    /// <param name="items">The page items.</param>
    /// <param name="limit">The requested page size, between 1 and <see cref="MaxLimit"/>.</param>
    /// <param name="nextCursor">A non-blank cursor for the next page.</param>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The limit is out of range.</exception>
    /// <exception cref="ArgumentException">The cursor is blank or the item count exceeds the limit.</exception>
    public static PagedResult<T> WithMore(IEnumerable<T> items, int limit, string nextCursor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nextCursor);
        return Create(items, limit, nextCursor, hasMore: true);
    }

    /// <summary>Creates the final page (no further data, no cursor).</summary>
    /// <param name="items">The page items; may be empty.</param>
    /// <param name="limit">The requested page size, between 1 and <see cref="MaxLimit"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The limit is out of range.</exception>
    /// <exception cref="ArgumentException">The item count exceeds the limit.</exception>
    public static PagedResult<T> Last(IEnumerable<T> items, int limit)
        => Create(items, limit, nextCursor: null, hasMore: false);

    /// <summary>Creates a page from explicit pagination state, validating that it is consistent.</summary>
    /// <param name="items">The page items.</param>
    /// <param name="limit">The requested page size, between 1 and <see cref="MaxLimit"/>.</param>
    /// <param name="nextCursor">Required (non-blank) when <paramref name="hasMore"/> is true; must be null otherwise.</param>
    /// <param name="hasMore">Whether more data exists.</param>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The limit is out of range.</exception>
    /// <exception cref="ArgumentException">The cursor and <paramref name="hasMore"/> disagree, or the item count exceeds the limit.</exception>
    public static PagedResult<T> Create(IEnumerable<T> items, int limit, string? nextCursor, bool hasMore)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(limit, MaxLimit);

        if (hasMore && string.IsNullOrWhiteSpace(nextCursor))
        {
            throw new ArgumentException("A next cursor is required when more data exists.", nameof(nextCursor));
        }

        if (!hasMore && nextCursor is not null)
        {
            throw new ArgumentException("The next cursor must be null when no more data exists.", nameof(nextCursor));
        }

        T[] snapshot = items.ToArray();
        if (snapshot.Length > limit)
        {
            throw new ArgumentException(
                $"The page contains {snapshot.Length} items, which exceeds the limit of {limit}.",
                nameof(items));
        }

        return new PagedResult<T>(snapshot, limit, nextCursor, hasMore);
    }
}
