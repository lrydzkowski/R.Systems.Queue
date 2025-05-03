using Azure.Messaging.ServiceBus;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

public interface IMessageConsumer
{
    public Task ConsumeMessageAsync(ProcessMessageEventArgs args, IMessageSerializer messageSerializer);
}

public abstract class TypedMessageConsumer<TData> : IMessageConsumer
{
    public async Task ConsumeMessageAsync(ProcessMessageEventArgs args, IMessageSerializer messageSerializer)
    {
        TData? data = messageSerializer.Deserialize<TData>(args.Message.Body);
        if (data is null)
        {
            throw new InvalidOperationException("An unexpected error has occurred in deserializing the message");
        }

        await ConsumeMessageAsync(data);
    }

    public abstract Task ConsumeMessageAsync(TData data);
}
