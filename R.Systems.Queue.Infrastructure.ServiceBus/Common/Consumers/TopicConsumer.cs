using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

internal class TopicConsumer<TOptions, TListener> : ServiceBusConsumer<TListener>
    where TOptions : class, ITopicOptions, new()
    where TListener : class, IMessageConsumer
{
    private readonly TOptions _options;

    public TopicConsumer(
        IOptions<TOptions> options,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        TListener listener,
        string serviceBusClientName,
        ServiceBusProcessorOptions processorOptions
    ) : base(serviceBusClientFactory, listener, serviceBusClientName, processorOptions)
    {
        _options = options.Value;
    }

    protected override ServiceBusProcessor CreateProcessor()
    {
        return ServiceBusClient.CreateProcessor(_options.TopicName, _options.SubscriptionName, ProcessorOptions);
    }
}
