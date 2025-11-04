using Core.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Extensions;

public static class ProgramExtension
{
    public static IServiceCollection AddFluentValidationSupport(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

        ValidatorOptions.Global.LanguageManager.Enabled = false;
        ValidatorOptions.Global.DisplayNameResolver = (_, memberInfo, _) => memberInfo.Name.ToCamelCase();
        ValidatorOptions.Global.PropertyNameResolver = (_, memberInfo, _) => memberInfo.Name.ToCamelCase();

        return serviceCollection;
    }

    public static IServiceCollection AddMediatRSupport(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

            x.AddOpenBehavior(typeof(ExceptionBehavior<,>));
            x.AddOpenBehavior(typeof(LoggingBehavior<,>));
            x.AddOpenBehavior(typeof(PerformanceBehavior<,>));
        });

        return serviceCollection;
    }
}