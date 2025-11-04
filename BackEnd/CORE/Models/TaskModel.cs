namespace CORE.Models;

public class TaskModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public Guid AssigneeId { get; set; }
    public Guid AssignerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime Deadline {  get; set; }
    public int EstimatedHours { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string UpdatedBy {  get; set; } = string.Empty;  
    public DateTime UpdatedAt { get; set; }
}
