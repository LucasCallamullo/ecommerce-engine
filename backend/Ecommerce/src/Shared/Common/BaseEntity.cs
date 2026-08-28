namespace Ecommerce.Shared.Common;

// Non-generic interface for entity auditing and soft delete state.
// Why this exists:
// In C#, BaseEntity<int> and BaseEntity<long> (or Guid/string) are invariant, distinct types.
// EF Core's ChangeTracker cannot query 'BaseEntity<object>' to catch all modified entities.
// Implementing this interface allows AppDbContext.SaveChangesAsync to intercept and update audit fields
// (CreatedAt, UpdatedAt, IsDeleted) across ALL entities in a single generic loop, regardless of their primary key type.
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
    bool IsDeleted { get; set; }
}

// Abstract generic base class for domain entities.
// Why Generics (TKey):
// Allows child entities to specify their own primary key data type:
// - int / long: Auto-incrementing numerical IDs (standard for catalogues and high-volume transactions).
// - Guid: Globally Unique Identifiers (ideal for distributed systems or public-facing IDs).
// - string: Alphanumeric keys (e.g., ISO country codes, external slugs, or legacy system IDs).
public abstract class BaseEntity<TKey> : IAuditableEntity
{
    // Primary key of generic type TKey. Default! prevents nullability warnings for non-nullable types.
    public TKey Id { get; set; } = default!;

    // Timestamp when the record was initially created (stored in UTC).
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Timestamp of the last modification. Null until the entity is updated for the first time.
    public DateTime? UpdatedAt { get; set; }

    // Soft delete flag. When set to true, the record is logically deleted without removing the row from the database.
    public bool IsDeleted { get; set; } = false;
}