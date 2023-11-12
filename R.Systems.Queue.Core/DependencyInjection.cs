using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Queue.Core.Common.Validation;

namespace R.Systems.Queue.Core;

public static class DependencyInjection
{
    public static void ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddMediatR();
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection), includeInternalTypes: true);
    }

    public static void ConfigureOptionsWithValidation<TOptions, TValidator>(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationPosition
    )
        where TOptions : class
        where TValidator : class, IValidator<TOptions>, new()
    {
        services.AddSingleton<IValidator<TOptions>, TValidator>();
        services.AddOptions<TOptions>()
            .Bind(configuration.GetSection(configurationPosition))
            .ValidateFluently()
            .ValidateOnStart();
    }

    public static void ConfigureOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationPosition
    ) where TOptions : class
    {
        services.AddOptions<TOptions>().Bind(configuration.GetSection(configurationPosition));
    }

    private static void AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}
