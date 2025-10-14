
namespace CORE.Entities;

public class UserEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public string? Token { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public ICollection<UserRoleEntity>? UserRoles { get; set; }
    public ICollection<ProjectMemberEntity>? ProjectMembers { get; set; }
    public ICollection<TaskCommentEntity>? TaskComments { get; set; }
    public ICollection<NotificationEntity>? Notifications { get; set; }
}
