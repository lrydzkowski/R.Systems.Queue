using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Queue.Core;
using R.Systems.Queue.Core.Commands.SendCompanyToQueue;
using R.Systems.Queue.Core.Commands.SendCompanyToTopic;
using R.Systems.Queue.Infrastructure.ServiceBus.Common;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;
using R.Systems.Queue.Infrastructure.ServiceBus.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Senders;

namespace R.Systems.Queue.Infrastructure.ServiceBus;

public static class DependencyInjection
{
    public static void ConfigureServiceBusServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureServiceBusCommonServices();
        services.ConfigureOptions(configuration);
        services.ConfigureSenders();
        services.ConfigureInfrastructureManagers();
    }

    private static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<CompanyQueueOptions>(
            configuration,
            $"{ServiceBusOptions.Position}:{CompanyQueueOptions.Position}"
        );
        services.ConfigureOptions<CompanyTopicOptions>(
            configuration,
            $"{ServiceBusOptions.Position}:{CompanyTopicOptions.Position}"
        );
    }

    private static void ConfigureSenders(this IServiceCollection services)
    {
        services
            .ConfigureServiceBusQueueSender<ICompanyQueueSender, CompanyQueueSender, CompanyQueueMessage,
                CompanyQueueOptions>();
        services
            .ConfigureServiceBusTopicSender<ICompanyTopicSender, CompanyTopicSender, CompanyTopicMessage,
                CompanyTopicOptions>();
    }

    private static void ConfigureInfrastructureManagers(this IServiceCollection services)
    {
        services.AddSingleton<IQueueInfrastructureManager, QueueInfrastructureManager<CompanyQueueOptions>>();
        services.AddSingleton<ITopicInfrastructureManager, TopicInfrastructureManager<CompanyTopicOptions>>();
    }
}
