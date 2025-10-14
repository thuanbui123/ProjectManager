namespace CORE.Entities;

public class NotificationEntity
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }

    [MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? RelatedObjectType { get; set; }
    public Guid? RelatedObjectId { get; set; }

    public bool IsSystem { get; set; } = false;
}
