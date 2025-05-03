using Azure.Messaging.ServiceBus;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

public interface IMessageConsumer
{
    public Task ProcessMessageAsync(ProcessMessageEventArgs args, IMessageSerializer messageSerializer);
}

public abstract class TypedMessageConsumer<TData> : IMessageConsumer
{
    public async Task ProcessMessageAsync(ProcessMessageEventArgs args, IMessageSerializer messageSerializer)
    {
        TData? data = messageSerializer.Deserialize<TData>(args.Message.Body);
        if (data is null)
        {
            throw new InvalidOperationException("An unexpected error has occurred in deserializing the message");
        }

        await ProcessMessageAsync(data);
    }

    public abstract Task ProcessMessageAsync(TData data);
}
