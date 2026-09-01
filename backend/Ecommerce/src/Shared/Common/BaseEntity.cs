namespace Ecommerce.Shared.Common;

/// <summary>
/// Defines a non-generic contract for entity auditing and soft delete state tracking.
/// </summary>
/// <remarks>
/// This non-generic abstraction allows <c>DbContext.SaveChangesAsync</c> to query the 
/// <c>ChangeTracker</c> for all modified entities in a single loop, bypassing C# generic variance 
/// limitations across distinct primary key types (e.g., <c>int</c>, <c>Guid</c>, <c>string</c>).
/// </remarks>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets or sets the UTC timestamp when the entity record was created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity record was last modified.
    /// </summary>
    DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is logically deleted.
    /// </summary>
    bool IsDeleted { get; set; }
}

/// <summary>
/// Represents the abstract generic base class for all domain entities within the system.
/// </summary>
/// <typeparam name="TKey">
/// The data type of the entity's primary key (e.g., <see cref="int"/>, <see cref="Guid"/>, <see cref="string"/>).
/// </typeparam>
public abstract class BaseEntity<TKey> : IAuditableEntity
{
    /// <summary>
    /// Gets or sets the unique primary key identifier for the entity.
    /// </summary>
    public TKey Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the UTC timestamp indicating when the record was initially created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp of the last modification. Remains <c>null</c> until updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the record is logically soft-deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}