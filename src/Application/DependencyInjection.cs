using System.Reflection;
using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // Commands
        RegisterHandlers(services);

        // Validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void RegisterHandlers(IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            if (type.IsClass && !type.IsAbstract)
            {
                var interfaces = type.GetInterfaces();

                foreach (var interfaceType in interfaces)
                {
                    if (interfaceType.IsGenericType)
                    {
                        var genericDefinition = interfaceType.GetGenericTypeDefinition();

                        if (genericDefinition == typeof(ICommandHandler<>) ||
                            genericDefinition == typeof(ICommandHandler<,>) ||
                            genericDefinition == typeof(IQueryHandler<,>))
                        {
                            services.AddScoped(interfaceType, type);
                        }
                    }
                }
            }
        }
    }
}
