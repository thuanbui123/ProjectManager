namespace CORE.Entities;

public class TaskCommentEntity
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Task")]
    public Guid TaskId { get; set; }
    public TaskEntity? Task { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
