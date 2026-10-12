using FinSightIQ.BuildingBlocks.Entities;

namespace FinSightIQ.BuildingBlocks.Tests.Entities;

public class BaseEntityTests
{
    private sealed class TestEntity : BaseEntity
    {
        public TestEntity()
        {
        }

        public TestEntity(Guid id)
            : base(id)
        {
        }
    }

    [Fact]
    public void DefaultConstructor_GeneratesNonEmptyUniqueIds()
    {
        TestEntity first = new();
        TestEntity second = new();

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Constructor_WithValidId_PreservesId()
    {
        Guid id = Guid.NewGuid();

        TestEntity entity = new(id);

        Assert.Equal(id, entity.Id);
    }

    [Fact]
    public void Constructor_WithEmptyGuid_Throws()
    {
        Assert.Throws<ArgumentException>(() => new TestEntity(Guid.Empty));
    }
}
