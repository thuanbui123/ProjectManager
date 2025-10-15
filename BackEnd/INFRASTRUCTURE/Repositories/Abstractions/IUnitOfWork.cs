using CORE.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace INFRASTRUCTURE.Repositories.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IStoredProcedureExecutor StoredProcedures { get; }

    IRepository<UserEntity> Users { get; }
    IRepository<RoleEntity> Roles { get; }
    IRepository<UserRoleEntity> UserRoles { get; }
    IRepository<ProjectEntity> Projects { get; }
    IRepository<ProjectMemberEntity> ProjectMembers { get; }
    IRepository<TaskEntity> Tasks { get; }
    IRepository<TaskCommentEntity> TaskComments { get; }
    IRepository<TaskHistoryEntity> TaskHistories { get; }
    IRepository<AttachmentEntity> Attachments { get; }
    IRepository<NotificationEntity> Notifications { get; }

    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();

    Task<int> CompleteAsync();
}
