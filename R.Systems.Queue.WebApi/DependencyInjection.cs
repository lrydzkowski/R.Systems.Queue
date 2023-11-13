using Azure.Messaging.ServiceBus;
using R.Systems.Queue.Infrastructure.ServiceBus.Common;
using R.Systems.Queue.Infrastructure.ServiceBus.Options;
using R.Systems.Queue.WebApi.Listeners;

namespace R.Systems.Queue.WebApi;

public static class DependencyInjection
{
    public static void ConfigureWebApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.ConfigureSwagger();
        services.ConfigureServiceBusListeners();
    }

    private static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen();
    }

    private static void ConfigureServiceBusListeners(this IServiceCollection services)
    {
        services.ConfigureServiceBusQueueListener<CompanyQueueListener, CompanyQueueOptions>(
            new ServiceBusProcessorOptions
            {
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10),
                MaxConcurrentCalls = 1
            }
        );
        services.ConfigureServiceBusTopicListener<CompanyTopicListener, CompanyTopicOptions>(
            new ServiceBusProcessorOptions
            {
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10),
                MaxConcurrentCalls = 1
            }
        );
    }
}
