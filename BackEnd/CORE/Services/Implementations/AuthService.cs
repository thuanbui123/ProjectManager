using CORE.Abstractions;
using CORE.Commons;
using CORE.Entities;
using CORE.Models;
using CORE.Services.Abstractions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace CORE.Services.Implementations;

public class AuthService : IAuthService
{
    public readonly IRepository<UserEntity> _userRepository = null!;
    public readonly IStoredProcedureExecutor _storedProcedureExecutor = null!;
    public readonly IUnitOfWork _unitOfWork = null!;

    public AuthService(IRepository<UserEntity> userRepository, IStoredProcedureExecutor storedProcedureExecutor, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _storedProcedureExecutor = storedProcedureExecutor;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConfirmRegisterResultModel> ConfirmRegister(Guid userId, string token)
    {
        var rs = await _storedProcedureExecutor.QueryAsync<ConfirmRegisterResultModel>("ConfirmEmail", new
        {
            UserId = userId,
            Token = token
        });
        return rs.FirstOrDefault() ?? new ConfirmRegisterResultModel();
    }

    public async Task<LoginResponseModel?> Login(string email, string password)
    {
        var rs = await _storedProcedureExecutor.QueryAsync<LoginResponseModel>("LoginUser", new
        {
            Email = email
        });
        if(rs.FirstOrDefault()!.IsLogin)
        {
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, rs.FirstOrDefault()!.Password);
            if (!isPasswordValid)
            {
                rs.FirstOrDefault()!.IsLogin = false;
                rs.FirstOrDefault()!.Message = "Mật khẩu không đúng";
            }
            rs.FirstOrDefault()!.Password = "";
        }
        return rs.FirstOrDefault();
    }

    public async Task<(string, Guid?)> RegisterUser (string username, string email, string password, string confirmationToken)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        var rs = await _storedProcedureExecutor.QueryAsync<RegisterResponseModel>("RegisterAccount", new
        {
            Username = username,
            Email = email,
            Password = passwordHash,
            RoleName = Constants.USER,
            ConfirmationToken = confirmationToken
        });

        switch (rs.FirstOrDefault()!.ResultCode)
        {
            case -1:
                return ("Email đã được sử dụng để đăng ký tài khoản. Vui lòng nhập email khác", null);
            case 0:
                return ("Xảy ra lỗi khi đăng ký tài khoản. Vui lòng thử lại sau ít phút", null);
            case 1:
                return ("Đăng ký tài khoản thành công", rs.FirstOrDefault()!.UserId);
            default:
                return ("", null);
        }
    }
}
