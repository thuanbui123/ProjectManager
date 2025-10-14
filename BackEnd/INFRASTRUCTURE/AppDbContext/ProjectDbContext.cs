using CORE.Entities;
using Microsoft.EntityFrameworkCore;

namespace INFRASTRUCTURE.AppDbContext;

public class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<ProjectMemberEntity> ProjectMembers { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<TaskCommentEntity> TaskComments { get; set; }
    public DbSet<TaskHistoryEntity> TaskHistories { get; set; }
    public DbSet<AttachmentEntity> Attachments { get; set; }
    public DbSet<NotificationEntity> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== USER - ROLE =====
        modelBuilder.Entity<UserRoleEntity>()
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        modelBuilder.Entity<UserRoleEntity>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRoleEntity>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== PROJECT - MEMBERS =====
        modelBuilder.Entity<ProjectMemberEntity>()
            .HasIndex(pm => new { pm.ProjectId, pm.UserId })
            .IsUnique();

        modelBuilder.Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMembers)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== PROJECT - TASKS =====
        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Parent Task (subtask)
        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.ParentTask)
            .WithMany()
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        // Assignee / Assigner là ProjectMember
        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.Assignee)
            .WithMany(pm => pm.AssignedTasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.Assigner)
            .WithMany()
            .HasForeignKey(t => t.AssignerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== TASK COMMENTS =====
        modelBuilder.Entity<TaskCommentEntity>()
            .HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskCommentEntity>()
            .HasOne(c => c.User)
            .WithMany(u => u.TaskComments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== ATTACHMENTS =====
        modelBuilder.Entity<AttachmentEntity>()
            .HasOne(a => a.Task)
            .WithMany(t => t.Attachments)
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== TASK HISTORY =====
        modelBuilder.Entity<TaskHistoryEntity>()
            .HasOne(h => h.Task)
            .WithMany(t => t.History)
            .HasForeignKey(h => h.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== NOTIFICATIONS =====
        modelBuilder.Entity<NotificationEntity>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NotificationEntity>()
            .HasIndex(n => new { n.UserId, n.IsRead });
    }
}
