using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;
using R.Systems.Queue.Infrastructure.ServiceBus.Senders;

namespace R.Systems.Queue.WebApi.Consumers;

public class Company2QueueConsumer : TypedMessageConsumer<CompanyQueueMessage>
{
    private readonly ILogger<Company2QueueConsumer> _logger;

    public Company2QueueConsumer(ILogger<Company2QueueConsumer> logger)
    {
        _logger = logger;
    }

    public override Task ConsumeMessageAsync(CompanyQueueMessage data)
    {
        _logger.LogInformation("Company, id: {Id}, name: {Name}", data.Id, data.Name);

        return Task.CompletedTask;
    }
}
