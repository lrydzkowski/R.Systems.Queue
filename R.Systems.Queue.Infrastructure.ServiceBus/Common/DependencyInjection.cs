using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Senders;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;
using TMHE.PartsShop.Infrastructure.Azure.ServiceBus.Common.Extensions;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common;

public static class DependencyInjection
{
    public static void ConfigureServiceBusCommonServices(this IServiceCollection services)
    {
        services.ConfigureServices();
    }

    public static void ConfigureServiceBusQueueConsumer<TConsumer, TOptions>(
        this IServiceCollection services,
        ServiceBusProcessorOptions? processorOptions = null
    )
        where TConsumer : class, IMessageConsumer
        where TOptions : class, IQueueOptions, new()
    {
        string name = typeof(TConsumer).Name;
        services.ConfigureServiceBusClient<TOptions>(name);
        services.AddSingleton<IServiceBusConsumer>(serviceProvider =>
            ActivatorUtilities.CreateInstance<QueueConsumer<TOptions, TConsumer>>(
                serviceProvider,
                name,
                processorOptions ?? new ServiceBusProcessorOptions()
            )
        );
        services.AddScoped<TConsumer>();
    }

    public static void ConfigureServiceBusTopicConsumer<TConsumer, TOptions>(
        this IServiceCollection services,
        ServiceBusProcessorOptions? processorOptions = null
    )
        where TConsumer : class, IMessageConsumer
        where TOptions : class, ITopicOptions, new()
    {
        string name = typeof(TConsumer).Name;
        services.ConfigureServiceBusClient<TOptions>(name);
        services.AddSingleton<IServiceBusConsumer>(serviceProvider =>
            ActivatorUtilities.CreateInstance<TopicConsumer<TOptions, TConsumer>>(
                serviceProvider,
                name,
                processorOptions ?? new ServiceBusProcessorOptions()
            )
        );
        services.AddScoped<TConsumer>();
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

    public static void ConfigureQueueCreator<TOptions>(
        this IServiceCollection services,
        Func<IServiceProvider, CreateQueueOptions, CreateQueueOptions>? processCreateQueueOptions = null
    )
        where TOptions : class, IQueueOptions, new()
    {
        string name = typeof(TOptions).Name;
        services.ConfigureServiceBusAdministrationClient<TOptions>(name);
        services.AddSingleton<IInfrastructureManager>(serviceProvider =>
            {
                CreateQueueOptions createQueueOptions = serviceProvider.BuildCreateQueueOptions<TOptions>();
                if (processCreateQueueOptions is not null)
                {
                    createQueueOptions = processCreateQueueOptions(serviceProvider, createQueueOptions);
                }

                return ActivatorUtilities.CreateInstance<QueueInfrastructureManager<TOptions>>(
                    serviceProvider,
                    name,
                    createQueueOptions
                );
            }
        );
    }

    public static void ConfigureTopicCreator<TOptions>(
        this IServiceCollection services,
        Func<IServiceProvider, CreateTopicOptions, CreateTopicOptions>? processCreateTopicOptions = null
    )
        where TOptions : class, ITopicOptions, new()
    {
        string name = typeof(TOptions).Name;
        services.ConfigureServiceBusAdministrationClient<TOptions>(name);
        services.AddSingleton<IInfrastructureManager>(serviceProvider =>
            {
                CreateTopicOptions createTopicOptions = serviceProvider.BuildCreateTopicOptions<TOptions>();
                if (processCreateTopicOptions is not null)
                {
                    createTopicOptions = processCreateTopicOptions(
                        serviceProvider,
                        createTopicOptions
                    );
                }

                return ActivatorUtilities.CreateInstance<TopicInfrastructureManager<TOptions>>(
                    serviceProvider,
                    name,
                    createTopicOptions
                );
            }
        );
    }

    public static void ConfigureTopicSubscriptionCreator<TOptions>(
        this IServiceCollection services,
        Func<IServiceProvider, CreateSubscriptionOptions, CreateSubscriptionOptions>? processCreateSubscriptionOptions =
            null
    )
        where TOptions : class, ITopicOptions, new()
    {
        string name = typeof(TOptions).Name;
        services.ConfigureServiceBusAdministrationClient<TOptions>(name);
        services.AddSingleton<IInfrastructureManager>(serviceProvider =>
            {
                CreateSubscriptionOptions createSubscriptionOptions =
                    serviceProvider.BuildCreateSubscriptionOptions<TOptions>();
                if (processCreateSubscriptionOptions is not null)
                {
                    createSubscriptionOptions = processCreateSubscriptionOptions(
                        serviceProvider,
                        createSubscriptionOptions
                    );
                }

                return ActivatorUtilities.CreateInstance<TopicSubscriptionInfrastructureManager<TOptions>>(
                    serviceProvider,
                    name,
                    createSubscriptionOptions
                );
            }
        );
    }

    private static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IMessageSerializer, MessageSerializer>();
        services.AddSingleton<INamesResolver, NamesResolver>();
        services.AddHostedService<WorkerServiceBus>();
    }

    private static void ConfigureServiceBusClient<TOptions>(this IServiceCollection services, string name)
        where TOptions : class, IServiceBusOptions, new()
    {
        services.AddAzureClients(azureClientFactoryBuilder =>
            {
                azureClientFactoryBuilder.AddClient<ServiceBusClient, ServiceBusClientOptions>((
                            _,
                            serviceProvider
                        ) =>
                        {
                            TOptions options = serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;

                            return new ServiceBusClient(options.ConnectionString);
                        }
                    )
                    .WithName(name);
            }
        );
    }

    private static void ConfigureServiceBusAdministrationClient<TOptions>(this IServiceCollection services, string name)
        where TOptions : class, IServiceBusOptions, new()
    {
        services.AddAzureClients(azureClientFactoryBuilder =>
            {
                azureClientFactoryBuilder
                    .AddClient<ServiceBusAdministrationClient, ServiceBusAdministrationClientOptions>((
                            _,
                            serviceProvider
                        ) =>
                        {
                            TOptions options = serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;

                            return new ServiceBusAdministrationClient(options.ConnectionString);
                        }
                    )
                    .WithName(name);
            }
        );
    }
}
