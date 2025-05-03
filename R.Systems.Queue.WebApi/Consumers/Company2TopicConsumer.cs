using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;
using R.Systems.Queue.Infrastructure.ServiceBus.Senders;

namespace R.Systems.Queue.WebApi.Consumers;

public class Company2TopicConsumer : TypedMessageConsumer<CompanyTopicMessage>
{
    public Company2TopicConsumer(
        ILogger<Company2TopicConsumer> logger,
        IMessageSerializer messageSerializer
    ) : base(logger, messageSerializer)
    {
    }

    public override Task ProcessMessageAsync(CompanyTopicMessage data)
    {
        Logger.LogInformation("Company, id: {Id}, name: {Name}", data.Id, data.Name);

        return Task.CompletedTask;
    }
}
