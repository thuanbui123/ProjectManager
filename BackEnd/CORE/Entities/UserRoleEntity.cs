
namespace CORE.Entities;

public class UserRoleEntity
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }

    [ForeignKey("Role")]
    public Guid RoleId { get; set; }
    public RoleEntity? Role { get; set; }
}
