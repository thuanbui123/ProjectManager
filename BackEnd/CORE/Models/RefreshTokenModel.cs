namespace CORE.Models;

public class RefreshTokenModel
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime ExpiryDate { get; set; }
}
