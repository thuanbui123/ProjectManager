namespace CORE.Entities;

public class UserRefreshTokenEntity
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; } //Đánh dấu token bị thu hồi.
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedDate { get; set; }
    public string? DeviceInfo { get; set; }

    public UserEntity User { get; set; } = null!;
}
