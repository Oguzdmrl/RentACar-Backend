using Core.Application.Pipelines.Validation;
using Core.Application.Rules;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class ApplicationServicesRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules)); // Bu assembly'deki BaseBusinessRuless türündeki servisleri IOC'ye ekle.

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // validasyonlarımız



        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            configuration.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
        });
        return services;
    }

    public static IServiceCollection AddSubClassesOfType(
        this IServiceCollection services,
        Assembly assembly,
        Type type,
        Func<IServiceCollection, Type, IServiceCollection>? addWithlifeCycle = null
        )
    {
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
        foreach (var item in types)
            if (addWithlifeCycle == null)
                services.AddScoped(item);
            else
                addWithlifeCycle(services, type);
        return services;
    }


}
