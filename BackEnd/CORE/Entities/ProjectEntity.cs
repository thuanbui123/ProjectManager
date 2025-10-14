namespace CORE.Entities;

public class ProjectEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [MaxLength(100)]
    public string Status { get; set; } = "InProgress";

    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Optional: hỗ trợ phân cấp dự án
    public Guid? ParentProjectId { get; set; }
    public ProjectEntity? ParentProject { get; set; }

    // Navigation
    public ICollection<ProjectMemberEntity> Members { get; set; }
    public ICollection<TaskEntity> Tasks { get; set; }
}
