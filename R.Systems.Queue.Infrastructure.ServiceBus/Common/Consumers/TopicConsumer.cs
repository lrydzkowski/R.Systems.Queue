using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

internal class TopicConsumer<TOptions, TConsumer> : ServiceBusConsumer<TConsumer>
    where TOptions : class, ITopicOptions, new()
    where TConsumer : class, IMessageConsumer
{
    private readonly INamesResolver _namesResolver;
    private readonly TOptions _options;

    public TopicConsumer(
        IOptions<TOptions> options,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        TConsumer consumer,
        string serviceBusClientName,
        ServiceBusProcessorOptions processorOptions,
        INamesResolver namesResolver
    ) : base(serviceBusClientFactory, consumer, serviceBusClientName, processorOptions)
    {
        _namesResolver = namesResolver;
        _options = options.Value;
    }

    protected override ServiceBusProcessor? CreateProcessor()
    {
        return ServiceBusClient?.CreateProcessor(
            _namesResolver.ResolveTopicName(_options),
            _namesResolver.ResolveSubscriptionName(_options),
            ProcessorOptions
        );
    }
}
