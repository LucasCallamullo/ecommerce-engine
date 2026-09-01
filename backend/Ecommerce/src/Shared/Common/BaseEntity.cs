namespace Ecommerce.Shared.Common;


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