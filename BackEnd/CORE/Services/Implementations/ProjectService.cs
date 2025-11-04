using CORE.Abstractions;
using CORE.Entities;
using CORE.Models;
using CORE.Services.Abstractions;
using Newtonsoft.Json;

namespace CORE.Services.Implementations;

public class ProjectService : IProjectService
{
    private readonly IStoredProcedureExecutor _executor;
    private readonly IRepository<ProjectEntity> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IStoredProcedureExecutor executor, IRepository<ProjectEntity> repository, IUnitOfWork unitOfWork)
    {
        _executor = executor;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StoredProcedureResult> CreateOrUpdateProject(ProjectRequestModel projectRequest)
    {
        var rs = await _executor.QueryAsync<StoredProcedureResult>("CreateOrUpdateProject",
            new
            {
                Id = projectRequest.Id,
                Name = projectRequest.Name,
                Description = projectRequest.Description,
                StartDate = projectRequest.StartDate,
                EndDate = projectRequest.EndDate,
                Status = projectRequest.Status,
                User = projectRequest.Username,
                ParentProjectName = projectRequest.ParentProjectName
            }
        );
        return rs.FirstOrDefault() ?? new StoredProcedureResult();
    }

    public async Task<string> DeleteProject(Guid id, string username)
    {
        var project = await _repository.FindAsync(p => p.Id == id && !p.IsDeleted);
        if(project == null || !project.Any())
        {
            return "Dự án không tồn tại hoặc đã bị xóa";
        }
        var updateProject = project.FirstOrDefault();
        updateProject!.IsDeleted = true;
        updateProject!.UpdatedAt = DateTime.Now;
        updateProject!.UpdatedBy = username;
        _repository.Update(updateProject);
        await _unitOfWork.CompleteAsync();
        return "Xóa dự án thành công";
    }

    public async Task<IEnumerable<ProjectModel>> GetAllProjectByUsernameOrName(string username, string name)
    {
        var projects = await _executor.QueryAsync<ProjectModel>("GetAllProjectByUsernameOrName", new
        {
            Username = username,
            ProjectName = name
        });

        return projects;
    }

    public async Task<ProjectModel?> GetProjectWithTasks(Guid id)
    {
        var rs = await _executor.QueryAsync<JsonResultModel>("sp_GetProjectWithTasks", new
        {
            ProjectId = id
        });
        
        return JsonConvert.DeserializeObject<ProjectModel>(rs?.FirstOrDefault()?.Json ?? string.Empty);
    }
}

public class JsonResultModel
{
    public string Json { get; set; } = string.Empty;
}
