using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace TMHE.PartsShop.Infrastructure.Azure.ServiceBus.Common.Extensions;

internal static class ServiceProviderExtensions
{
    public static CreateQueueOptions BuildCreateQueueOptions<TOptions>(this IServiceProvider serviceProvider)
        where TOptions : class, IQueueOptions, new()
    {
        INamesResolver namesResolver = serviceProvider.GetRequiredService<INamesResolver>();
        IOptions<TOptions> options = serviceProvider.GetRequiredService<IOptions<TOptions>>();
        CreateQueueOptions createQueueOptions = new(namesResolver.ResolveQueueName(options.Value));

        return createQueueOptions;
    }

    public static CreateTopicOptions BuildCreateTopicOptions<TOptions>(
        this IServiceProvider serviceProvider
    )
        where TOptions : class, ITopicOptions, new()
    {
        INamesResolver namesResolver = serviceProvider.GetRequiredService<INamesResolver>();
        IOptions<TOptions> options = serviceProvider.GetRequiredService<IOptions<TOptions>>();
        CreateTopicOptions createTopicOptions = new(namesResolver.ResolveTopicName(options.Value));

        return createTopicOptions;
    }

    public static CreateSubscriptionOptions BuildCreateSubscriptionOptions<TOptions>(
        this IServiceProvider serviceProvider
    )
        where TOptions : class, ITopicOptions, new()
    {
        INamesResolver namesResolver = serviceProvider.GetRequiredService<INamesResolver>();
        IOptions<TOptions> options = serviceProvider.GetRequiredService<IOptions<TOptions>>();
        CreateSubscriptionOptions createSubscriptionOptions = new(
            namesResolver.ResolveTopicName(options.Value),
            namesResolver.ResolveSubscriptionName(options.Value)
        );

        return createSubscriptionOptions;
    }
}
