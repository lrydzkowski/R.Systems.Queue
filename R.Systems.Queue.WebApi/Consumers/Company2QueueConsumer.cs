using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;
using R.Systems.Queue.Infrastructure.ServiceBus.Senders;

namespace R.Systems.Queue.WebApi.Consumers;

public class Company2QueueConsumer : TypedMessageConsumer<CompanyQueueMessage>
{
    public Company2QueueConsumer(
        ILogger<Company2QueueConsumer> logger,
        IMessageSerializer messageSerializer
    ) : base(logger, messageSerializer)
    {
    }

    public override Task ProcessMessageAsync(CompanyQueueMessage data)
    {
        Logger.LogInformation("Company, id: {Id}, name: {Name}", data.Id, data.Name);

        return Task.CompletedTask;
    }
}
