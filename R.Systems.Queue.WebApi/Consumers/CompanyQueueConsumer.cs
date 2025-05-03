using Azure.Messaging.ServiceBus;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.WebApi.Consumers;

public class CompanyQueueConsumer : IMessageConsumer
{
    private readonly ILogger<CompanyQueueConsumer> _logger;

    public CompanyQueueConsumer(ILogger<CompanyQueueConsumer> logger)
    {
        _logger = logger;
    }

    public Task ConsumeMessageAsync(ProcessMessageEventArgs args, IMessageSerializer messageSerializer)
    {
        ServiceBusReceivedMessage? message = args.Message;
        string body = message.Body.ToString();
        _logger.LogInformation("Company queue consumer message: {Body}", body);

        return Task.CompletedTask;
    }
}
