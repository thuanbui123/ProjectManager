using CORE.Entities;
using INFRASTRUCTURE.AppDbContext;
using INFRASTRUCTURE.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace INFRASTRUCTURE.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly ProjectDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public IRepository<UserEntity> Users { get; }
    public IRepository<RoleEntity> Roles { get; }
    public IRepository<UserRoleEntity> UserRoles { get; }
    public IRepository<ProjectEntity> Projects { get; }
    public IRepository<ProjectMemberEntity> ProjectMembers { get; }
    public IRepository<TaskEntity> Tasks { get; }
    public IRepository<TaskCommentEntity> TaskComments { get; }
    public IRepository<TaskHistoryEntity> TaskHistories { get; }
    public IRepository<AttachmentEntity> Attachments { get; }
    public IRepository<NotificationEntity> Notifications { get; }

    public UnitOfWork(ProjectDbContext context)
    {
        _context = context;

        Users = new Repository<UserEntity>(context);
        Roles = new Repository<RoleEntity>(context);
        UserRoles = new Repository<UserRoleEntity>(context);
        Projects = new Repository<ProjectEntity>(context);
        ProjectMembers = new Repository<ProjectMemberEntity>(context);
        Tasks = new Repository<TaskEntity>(context);
        TaskComments = new Repository<TaskCommentEntity>(context);
        TaskHistories = new Repository<TaskHistoryEntity>(context);
        Attachments = new Repository<AttachmentEntity>(context);
        Notifications = new Repository<NotificationEntity>(context);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_currentTransaction != null)
            return _currentTransaction;

        _currentTransaction = await _context.Database.BeginTransactionAsync();
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }


    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
