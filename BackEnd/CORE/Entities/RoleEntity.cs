﻿
namespace CORE.Entities;

public class RoleEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<UserRoleEntity>? UserRoles { get; set; }
}
