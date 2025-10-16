namespace CORE.Models;

public class RegisterRequestModel
{
    [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Tên người dùng phải từ 5 đến 100 ký tự.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ 8 đến 100 ký tự.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    public string Email { get; set; } = string.Empty;
}
