using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

internal class QueueConsumer<TOptions, TListener> : ServiceBusConsumer<TListener>
    where TOptions : class, IQueueOptions, new()
    where TListener : class, IMessageConsumer
{
    private readonly TOptions _options;

    public QueueConsumer(
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
        return ServiceBusClient.CreateProcessor(_options.QueueName, ProcessorOptions);
    }
}
