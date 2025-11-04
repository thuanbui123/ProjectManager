using CORE.Models;

namespace CORE.Services.Abstractions;

public interface IAuthService
{
    public Task<LoginResponseModel?> Login(string email, string password);
    public Task<(string, Guid?)> RegisterUser(string username, string email, string password, string confirmationToken);
    public Task<StoredProcedureResult> ConfirmRegister(Guid userId, string token);
}
