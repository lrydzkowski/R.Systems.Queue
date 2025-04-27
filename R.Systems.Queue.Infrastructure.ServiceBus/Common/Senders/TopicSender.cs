using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Senders;

internal class TopicSender<TData, TOptions> : ServiceBusSenderBase<TData>
    where TData : class where TOptions : class, ITopicOptions, new()
{
    public TopicSender(
        IOptions<TOptions> options,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IMessageSerializer messageSerializer,
        string serviceBusClientName,
        INamesResolver namesResolver
    ) : base(serviceBusClientFactory, messageSerializer, serviceBusClientName)
    {
        ServiceBusSender = ServiceBusClient.CreateSender(namesResolver.ResolveTopicName(options.Value));
    }
}
