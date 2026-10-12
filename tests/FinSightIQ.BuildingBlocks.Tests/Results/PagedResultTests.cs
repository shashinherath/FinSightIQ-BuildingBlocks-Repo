using FinSightIQ.BuildingBlocks.Results;

namespace FinSightIQ.BuildingBlocks.Tests.Results;

public class PagedResultTests
{
    [Fact]
    public void WithMore_ValidPage_ExposesPaginationState()
    {
        PagedResult<int> page = PagedResult<int>.WithMore(new[] { 1, 2, 3 }, 3, "cursor-1");

        Assert.Equal(new[] { 1, 2, 3 }, page.Items);
        Assert.True(page.HasMore);
        Assert.Equal("cursor-1", page.NextCursor);
        Assert.Equal(3, page.Limit);
    }

    [Fact]
    public void Last_FinalPage_HasNoCursor()
    {
        PagedResult<int> page = PagedResult<int>.Last(new[] { 1 }, 50);

        Assert.False(page.HasMore);
        Assert.Null(page.NextCursor);
    }

    [Fact]
    public void Last_EmptyPage_IsValid()
    {
        PagedResult<string> page = PagedResult<string>.Last(Array.Empty<string>(), PagedResult<string>.DefaultLimit);

        Assert.Empty(page.Items);
        Assert.Equal(50, page.Limit);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(501)]
    public void Create_WithLimitOutOfRange_Throws(int limit)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PagedResult<int>.Last(Array.Empty<int>(), limit));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(500)]
    public void Create_WithBoundaryLimit_Succeeds(int limit)
    {
        PagedResult<int> page = PagedResult<int>.Last(Array.Empty<int>(), limit);

        Assert.Equal(limit, page.Limit);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_HasMoreWithoutCursor_Throws(string? cursor)
    {
        Assert.Throws<ArgumentException>(() => PagedResult<int>.Create(new[] { 1 }, 10, cursor, hasMore: true));
    }

    [Fact]
    public void WithMore_NullCursor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => PagedResult<int>.WithMore(new[] { 1 }, 10, null!));
    }

    [Fact]
    public void Create_CursorWithoutHasMore_Throws()
    {
        Assert.Throws<ArgumentException>(() => PagedResult<int>.Create(new[] { 1 }, 10, "cursor", hasMore: false));
    }

    [Fact]
    public void Create_NullItems_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => PagedResult<int>.Last(null!, 10));
    }

    [Fact]
    public void Create_MoreItemsThanLimit_Throws()
    {
        Assert.Throws<ArgumentException>(() => PagedResult<int>.Last(new[] { 1, 2, 3 }, 2));
    }

    [Fact]
    public void Create_SnapshotsItems_SoLaterSourceChangesDoNotLeak()
    {
        List<int> source = new() { 1, 2 };

        PagedResult<int> page = PagedResult<int>.Last(source, 10);
        source.Add(3);

        Assert.Equal(2, page.Items.Count);
    }
}
