using System.ComponentModel.DataAnnotations;

namespace BookingAPI.src.Shared.Utils;

public abstract class AuditableEntity
{
    // UUID por padrão
    public Guid Id { get; }

    // dados de auditoria
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    [Timestamp]
    public uint RowVersion { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public Guid? DeletedBy { get; set; }

    public void SoftDelete(Guid? deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = default;
        DeletedBy = default;
    }
}