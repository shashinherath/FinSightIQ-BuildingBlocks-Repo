namespace FinSightIQ.BuildingBlocks.Entities;

/// <summary>
/// Optional base class for entities identified by a <see cref="Guid"/>.
/// </summary>
/// <remarks>
/// Entities with other key strategies (for example surrogate integer keys) should not use this type.
/// It deliberately defines no audit or soft-delete fields; add those per entity where required.
/// </remarks>
public abstract class BaseEntity
{
    /// <summary>Initializes the entity with a newly generated identifier.</summary>
    protected BaseEntity()
        : this(Guid.NewGuid())
    {
    }

    /// <summary>Initializes the entity with an existing identifier (for example when rehydrating).</summary>
    /// <param name="id">A non-empty identifier.</param>
    /// <exception cref="ArgumentException"><paramref name="id"/> is <see cref="Guid.Empty"/>.</exception>
    protected BaseEntity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Entity identifier must not be empty.", nameof(id));
        }

        Id = id;
    }

    /// <summary>Gets the unique identifier of the entity.</summary>
    public Guid Id { get; }
}
