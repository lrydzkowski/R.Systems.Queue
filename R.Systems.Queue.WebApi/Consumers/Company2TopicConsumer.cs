using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;
using R.Systems.Queue.Infrastructure.ServiceBus.Senders;

namespace R.Systems.Queue.WebApi.Consumers;

public class Company2TopicConsumer : TypedMessageConsumer<CompanyTopicMessage>
{
    private readonly ILogger<Company2TopicConsumer> _logger;

    public Company2TopicConsumer(ILogger<Company2TopicConsumer> logger)
    {
        _logger = logger;
    }

    public override Task ConsumeMessageAsync(CompanyTopicMessage data)
    {
        _logger.LogInformation("Company, id: {Id}, name: {Name}", data.Id, data.Name);

        return Task.CompletedTask;
    }
}
