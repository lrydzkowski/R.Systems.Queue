using Azure.Messaging.ServiceBus;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

public interface IMessageConsumer
{
    public Task ProcessMessageAsync(ProcessMessageEventArgs args);

    public Task ProcessErrorAsync(ProcessErrorEventArgs arg);
}
