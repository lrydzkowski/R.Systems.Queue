using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Senders;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common;

public static class DependencyInjection
{
    public static void ConfigureServiceBusCommonServices(this IServiceCollection services)
    {
        services.ConfigureServices();
    }

    public static void ConfigureServiceBusQueueConsumer<TListener, TOptions>(
        this IServiceCollection services,
        ServiceBusProcessorOptions? processorOptions = null
    ) where TListener : class, IMessageConsumer where TOptions : class, IQueueOptions, new()
    {
        string name = typeof(TListener).Name;
        services.ConfigureServiceBusClient<TOptions>(name);
        services.AddSingleton<IServiceBusConsumer>(serviceProvider =>
            ActivatorUtilities.CreateInstance<QueueConsumer<TOptions, TListener>>(
                serviceProvider,
                name,
                processorOptions ?? new ServiceBusProcessorOptions()
            )
        );
        services.AddSingleton<TListener>();
    }

    public static void ConfigureServiceBusTopicConsumer<TListener, TOptions>(
        this IServiceCollection services,
        ServiceBusProcessorOptions? processorOptions = null
    ) where TListener : class, IMessageConsumer where TOptions : class, ITopicOptions, new()
    {
        string name = typeof(TListener).Name;
        services.ConfigureServiceBusClient<TOptions>(name);
        services.AddSingleton<IServiceBusConsumer>(serviceProvider =>
            ActivatorUtilities.CreateInstance<TopicConsumer<TOptions, TListener>>(
                serviceProvider,
                name,
                processorOptions ?? new ServiceBusProcessorOptions()
            )
        );
        services.AddSingleton<TListener>();
    }

    public static void ConfigureServiceBusQueueSender<TSender, TSenderImplementation, TData, TOptions>(
        this IServiceCollection services
    )
        where TSender : class
        where TSenderImplementation : class, TSender
        where TData : class
        where TOptions : class, IQueueOptions, new()
    {
        string name = typeof(TSender).Name;
        services.ConfigureServiceBusClient<TOptions>(name);
        services.AddSingleton<IServiceBusSender<TData>>(serviceProvider =>
            ActivatorUtilities.CreateInstance<QueueSender<TData, TOptions>>(serviceProvider, name)
        );
        services.AddScoped<TSender, TSenderImplementation>();
    }

    public static void ConfigureServiceBusTopicSender<TSender, TSenderImplementation, TData, TOptions>(
        this IServiceCollection services
    )
        where TSender : class
        where TSenderImplementation : class, TSender
        where TData : class
        where TOptions : class, ITopicOptions, new()
    {
        string name = typeof(TSender).Name;
        services.ConfigureServiceBusClient<TOptions>(name);
        services.AddSingleton<IServiceBusSender<TData>>(serviceProvider =>
            ActivatorUtilities.CreateInstance<TopicSender<TData, TOptions>>(serviceProvider, name)
        );
        services.AddScoped<TSender, TSenderImplementation>();
    }

    private static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IMessageSerializer, MessageSerializer>();
        services.AddHostedService<WorkerServiceBus>();
    }

    private static void ConfigureServiceBusClient<TOptions>(this IServiceCollection services, string name)
        where TOptions : class, IServiceBusOptions, new()
    {
        services.AddAzureClients(azureClientFactoryBuilder =>
            {
                azureClientFactoryBuilder.AddClient<ServiceBusClient, ServiceBusClientOptions>((_, serviceProvider) =>
                        {
                            TOptions options = serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;

                            return new ServiceBusClient(options.ConnectionString);
                        }
                    )
                    .WithName(name);
            }
        );
    }
}
