using INFRASTRUCTURE.Externals;
using INFRASTRUCTURE.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace INFRASTRUCTURE;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure (this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        return services;
    }
}
