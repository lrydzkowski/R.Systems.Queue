using Azure.Messaging.ServiceBus;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

namespace R.Systems.Queue.WebApi.Consumers;

public class CompanyTopicConsumer : IMessageConsumer
{
    private readonly ILogger<CompanyTopicConsumer> _logger;

    public CompanyTopicConsumer(ILogger<CompanyTopicConsumer> logger)
    {
        _logger = logger;
    }

    public Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        ServiceBusReceivedMessage? message = args.Message;
        string body = message.Body.ToString();
        _logger.LogInformation("Company topic consumer message: {Body}", body);

        return Task.CompletedTask;
    }

    public Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "An unexpected error has occurred in the company topic consumer");

        return Task.CompletedTask;
    }
}
