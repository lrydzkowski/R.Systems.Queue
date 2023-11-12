using Azure.Messaging.ServiceBus;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Listeners;

public interface IMessageListener
{
    public Task ProcessMessageAsync(ProcessMessageEventArgs args);

    public Task ProcessErrorAsync(ProcessErrorEventArgs arg);
}
