namespace CORE.Entities;

public class ProjectMemberEntity
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Project")]
    public Guid ProjectId { get; set; }
    public ProjectEntity? Project { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }

    [MaxLength(50)]
    public string RoleInProject { get; set; } = string.Empty;

    public string? CustomPermissions { get; set; } // JSON string

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    // Navigation
    public ICollection<TaskEntity>? AssignedTasks { get; set; }
}
