using Domain.Repos;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repos;

namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistence(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IAccountRepository, AccountRepository>();
    }
}
