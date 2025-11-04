using CORE.Entities;
using CORE.Models;

namespace CORE.Services.Abstractions;

public interface IProjectService
{
    public Task<StoredProcedureResult> CreateOrUpdateProject(ProjectRequestModel projectRequest);
    public Task<string> DeleteProject(Guid id, string username);
    public Task<IEnumerable<ProjectModel>> GetAllProjectByUsernameOrName(string username, string name);
    public Task<ProjectModel?> GetProjectWithTasks(Guid id);
}
