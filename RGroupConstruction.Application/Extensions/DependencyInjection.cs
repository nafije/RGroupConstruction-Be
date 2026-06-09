using RGroupConstruction.Application.Behavior;
using RGroupConstruction.Application.Dependency;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Mappings;
using RGroupConstruction.Application.Services;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace RGroupConstruction.Application.Extensions;


public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddApplicationCommon();
        services.AddSingleton<IMessageLocalizer, MessageLocalizer>();
        services.AddSingleton<IMapper>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var expression = new MapperConfigurationExpression();
            expression.AddProfile<MappingProfile>();
            var config = new MapperConfiguration(expression, loggerFactory);
            return config.CreateMapper();
        });

        // Add MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        // Add MediatR Pipeline Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddApplicationValidators(assembly);

        return services;
    }

    private static IServiceCollection AddApplicationValidators(
        this IServiceCollection services,
        Assembly assembly)
    {
        var validatorInterface = typeof(IValidator<>);
        var validatorTypes = assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition)
            .Select(type => new
            {
                ImplementationType = type,
                ServiceTypes = type.GetInterfaces()
                    .Where(interfaceType => interfaceType.IsGenericType &&
                                            interfaceType.GetGenericTypeDefinition() == validatorInterface)
                    .ToList()
            })
            .Where(type => type.ServiceTypes.Count > 0);

        foreach (var validatorType in validatorTypes)
        {
            foreach (var serviceType in validatorType.ServiceTypes)
                services.AddTransient(serviceType, validatorType.ImplementationType);
        }

        return services;
    }
}


