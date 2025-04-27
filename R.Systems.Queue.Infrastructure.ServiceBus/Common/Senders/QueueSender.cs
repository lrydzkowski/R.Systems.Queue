using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Senders;

internal class QueueSender<TData, TOptions> : ServiceBusSenderBase<TData>
    where TData : class where TOptions : class, IQueueOptions, new()
{
    public QueueSender(
        IOptions<TOptions> options,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IMessageSerializer messageSerializer,
        string serviceBusClientName
    ) : base(serviceBusClientFactory, messageSerializer, serviceBusClientName)
    {
        ServiceBusSender = ServiceBusClient.CreateSender(options.Value.QueueName);
    }
}
