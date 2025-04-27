using Azure.Messaging.ServiceBus;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

namespace R.Systems.Queue.WebApi.Consumers;

public class CompanyQueueConsumer : IMessageConsumer
{
    private readonly ILogger<CompanyQueueConsumer> _logger;

    public CompanyQueueConsumer(ILogger<CompanyQueueConsumer> logger)
    {
        _logger = logger;
    }

    public Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        ServiceBusReceivedMessage? message = args.Message;
        string body = message.Body.ToString();
        _logger.LogInformation("Company queue consumer message: {Body}", body);

        return Task.CompletedTask;
    }

    public Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "An unexpected error has occurred in the company queue consumer");

        return Task.CompletedTask;
    }
}
