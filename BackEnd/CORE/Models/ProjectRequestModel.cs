namespace CORE.Models;

public class ProjectRequestModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Chờ bắt đầu";
    public string ParentProjectName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
