using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

public abstract class TypedMessageConsumer<TData> : IMessageConsumer
{
    private readonly IMessageSerializer _messageSerializer;
    protected readonly ILogger<TypedMessageConsumer<TData>> Logger;

    protected TypedMessageConsumer(ILogger<TypedMessageConsumer<TData>> logger, IMessageSerializer messageSerializer)
    {
        Logger = logger;
        _messageSerializer = messageSerializer;
    }

    public async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        TData? data = _messageSerializer.Deserialize<TData>(args.Message.Body);
        if (data is null)
        {
            throw new InvalidOperationException("An unexpected error has occurred in deserializing the message");
        }

        await ProcessMessageAsync(data);
    }

    public Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        Logger.LogError(arg.Exception, "An unexpected error has occurred in the company queue consumer");

        return Task.CompletedTask;
    }

    public abstract Task ProcessMessageAsync(TData data);
}
