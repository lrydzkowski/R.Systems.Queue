using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

internal class TopicConsumer<TOptions, TConsumer> : ServiceBusConsumer<TConsumer>
    where TOptions : class, ITopicOptions, new()
    where TConsumer : class, IMessageConsumer
{
    private readonly TOptions _options;

    public TopicConsumer(
        IOptions<TOptions> options,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        TConsumer consumer,
        string serviceBusClientName,
        ServiceBusProcessorOptions processorOptions
    ) : base(serviceBusClientFactory, consumer, serviceBusClientName, processorOptions)
    {
        _options = options.Value;
    }

    protected override ServiceBusProcessor CreateProcessor()
    {
        return ServiceBusClient.CreateProcessor(_options.TopicName, _options.SubscriptionName, ProcessorOptions);
    }
}
