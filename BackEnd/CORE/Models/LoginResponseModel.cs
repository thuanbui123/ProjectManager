namespace CORE.Models;

public class LoginResponseModel
{
    public Guid UserId { get; set; }
    public string Username {  get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsLogin { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
