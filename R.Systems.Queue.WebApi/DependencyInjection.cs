﻿using Azure.Messaging.ServiceBus;
using R.Systems.Queue.Infrastructure.ServiceBus.Common;
using R.Systems.Queue.Infrastructure.ServiceBus.Options;
using R.Systems.Queue.WebApi.Consumers;

namespace R.Systems.Queue.WebApi;

public static class DependencyInjection
{
    public static void ConfigureWebApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.ConfigureSwagger();
        services.ConfigureServiceBusConsumers();
    }

    private static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen();
    }

    private static void ConfigureServiceBusConsumers(this IServiceCollection services)
    {
        services.ConfigureServiceBusQueueConsumer<CompanyQueueConsumer, CompanyQueueOptions>(
            new ServiceBusProcessorOptions
            {
                MaxAutoLockRenewalDuration = TimeSpan.FromHours(1),
                MaxConcurrentCalls = 1
            }
        );
        services.ConfigureServiceBusQueueConsumer<Company2QueueConsumer, Company2QueueOptions>(
            new ServiceBusProcessorOptions
            {
                MaxAutoLockRenewalDuration = TimeSpan.FromHours(1),
                MaxConcurrentCalls = 1
            }
        );
        services.ConfigureServiceBusTopicConsumer<CompanyTopicConsumer, CompanyTopicOptions>(
            new ServiceBusProcessorOptions
            {
                MaxAutoLockRenewalDuration = TimeSpan.FromHours(1),
                MaxConcurrentCalls = 1
            }
        );
        services.ConfigureServiceBusTopicConsumer<Company2TopicConsumer, Company2TopicOptions>(
            new ServiceBusProcessorOptions
            {
                MaxAutoLockRenewalDuration = TimeSpan.FromHours(1),
                MaxConcurrentCalls = 1
            }
        );
    }
}
