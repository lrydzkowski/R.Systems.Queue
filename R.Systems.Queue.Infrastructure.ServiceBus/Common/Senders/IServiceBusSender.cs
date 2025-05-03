using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Models;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Senders;

public interface IServiceBusSender<TData> : IAsyncDisposable
{
    public Task<SenderResult> SendAsync(
        TData data,
        Func<TData, ServiceBusMessage>? processor = null,
        CancellationToken cancellationToken = default
    );

    public Task<SenderResult> SendAsync(
        IReadOnlyCollection<TData> dataCollection,
        Func<TData, ServiceBusMessage>? processor = null,
        CancellationToken cancellationToken = default
    );
}

internal abstract class ServiceBusSenderBase<TData> : IServiceBusSender<TData>
    where TData : class
{
    private readonly IMessageSerializer _messageSerializer;
    protected readonly ServiceBusClient ServiceBusClient;
    protected ServiceBusSender? ServiceBusSender;

    protected ServiceBusSenderBase(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IMessageSerializer messageSerializer,
        string serviceBusClientName
    )
    {
        ServiceBusClient = serviceBusClientFactory.CreateClient(serviceBusClientName);
        _messageSerializer = messageSerializer;
    }

    public async Task<SenderResult> SendAsync(
        TData data,
        Func<TData, ServiceBusMessage>? processor = null,
        CancellationToken cancellationToken = default
    )
    {
        return await SendAsync([data], processor, cancellationToken);
    }

    public async Task<SenderResult> SendAsync(
        IReadOnlyCollection<TData> dataCollection,
        Func<TData, ServiceBusMessage>? processor = null,
        CancellationToken cancellationToken = default
    )
    {
        if (ServiceBusSender is null)
        {
            throw new InvalidOperationException("ServiceBusSender is null");
        }

        List<ServiceBusMessage> messages = [];
        foreach (TData data in dataCollection)
        {
            ServiceBusMessage message = processor is null
                ? new ServiceBusMessage(_messageSerializer.Serialize(data))
                : processor(data);
            messages.Add(message);
        }

        using ServiceBusMessageBatch messageBatch = await ServiceBusSender.CreateMessageBatchAsync(cancellationToken);
        foreach (ServiceBusMessage message in messages)
        {
            bool addMessageResult = messageBatch.TryAddMessage(message);
            if (!addMessageResult)
            {
                throw new InvalidOperationException(
                    "An unexpected error has occurred in adding a message to Service Bus."
                );
            }
        }

        await ServiceBusSender.SendMessagesAsync(messageBatch, cancellationToken);

        return new SenderResult
        {
            MessageIds = messages.ConvertAll(message => message.MessageId)
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (ServiceBusSender is not null)
        {
            await ServiceBusSender.DisposeAsync();
        }

        await ServiceBusClient.DisposeAsync();
    }
}
