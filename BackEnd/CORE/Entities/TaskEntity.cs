using System.Net.Mail;

namespace CORE.Entities;

public class TaskEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(100)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Priority { get; set; } = string.Empty;

    public Guid AssigneeId { get; set; }
    public ProjectMemberEntity? Assignee { get; set; }

    public Guid AssignerId { get; set; }
    public ProjectMemberEntity? Assigner { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? CompletedAt { get; set; }

    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    public Guid ProjectId { get; set; }
    public ProjectEntity? Project { get; set; }

    public Guid? ParentTaskId { get; set; }
    public TaskEntity? ParentTask { get; set; }

    // Navigation
    public ICollection<TaskCommentEntity>? Comments { get; set; }
    public ICollection<AttachmentEntity>? Attachments { get; set; }
    public ICollection<TaskHistoryEntity>? History { get; set; }
}
