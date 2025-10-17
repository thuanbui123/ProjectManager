namespace CORE.Models;

public class ServiceResultModel
{
    public bool Susscess { get; set; }
    public object? Data { get; set; }
    public List<String> Errors { get; set; } = new List<String>();
}
