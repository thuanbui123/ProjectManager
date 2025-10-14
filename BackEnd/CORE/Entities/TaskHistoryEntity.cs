namespace CORE.Entities;

public class TaskHistoryEntity
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Task")]
    public Guid TaskId { get; set; }
    public TaskEntity? Task { get; set; }

    [MaxLength(150)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(150)]
    public string UpdatedBy { get; set; } = string.Empty;

    public string? OldValue { get; set; } // JSON
    public string? NewValue { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
