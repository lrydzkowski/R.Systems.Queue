using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Models;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Senders;

public interface IServiceBusSender<TData> : IAsyncDisposable
{
    public Task<SenderResult> EnqueueAsync(TData data, Func<TData, ServiceBusMessage>? processor = null);

    public Task<SenderResult> EnqueueAsync(
        IReadOnlyCollection<TData> dataCollection,
        Func<TData, ServiceBusMessage>? processor = null
    );
}

internal abstract class ServiceBusSenderBase<TData> : IServiceBusSender<TData>
    where TData : class
{
    protected readonly ServiceBusClient ServiceBusClient;
    protected ServiceBusSender? ServiceBusSender;
    private readonly IMessageSerializer _messageSerializer;

    protected ServiceBusSenderBase(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IMessageSerializer messageSerializer,
        string serviceBusClientName
    )
    {
        ServiceBusClient = serviceBusClientFactory.CreateClient(serviceBusClientName);
        _messageSerializer = messageSerializer;
    }

    public async Task<SenderResult> EnqueueAsync(TData data, Func<TData, ServiceBusMessage>? processor = null)
    {
        return await EnqueueAsync(new List<TData> { data }, processor);
    }

    public async Task<SenderResult> EnqueueAsync(
        IReadOnlyCollection<TData> dataCollection,
        Func<TData, ServiceBusMessage>? processor = null
    )
    {
        if (ServiceBusSender == null)
        {
            throw new InvalidOperationException("ServiceBusSender is null");
        }

        List<ServiceBusMessage> messages = new();
        foreach (TData data in dataCollection)
        {
            ServiceBusMessage message = processor == null
                ? new ServiceBusMessage(_messageSerializer.Serialize(data))
                : processor(data);
            messages.Add(message);
        }

        using ServiceBusMessageBatch messageBatch = await ServiceBusSender.CreateMessageBatchAsync();
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

        await ServiceBusSender.SendMessagesAsync(messageBatch);

        return new SenderResult
        {
            MessageIds = messages.ConvertAll(x => x.MessageId)
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (ServiceBusSender != null)
        {
            await ServiceBusSender.DisposeAsync();
        }

        await ServiceBusClient.DisposeAsync();
    }
}
