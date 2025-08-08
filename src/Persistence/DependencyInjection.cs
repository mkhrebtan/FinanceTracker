using Domain.Repos;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repos;

namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddSingleton<IAccountRepository, AccountRepository>();
    }
}
