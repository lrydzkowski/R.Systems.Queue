using Azure.Messaging.ServiceBus;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Listeners;

namespace R.Systems.Queue.WebApi.Listeners;

public class CompanyQueueListener : IMessageListener
{
    private readonly ILogger<CompanyQueueListener> _logger;

    public CompanyQueueListener(ILogger<CompanyQueueListener> logger)
    {
        _logger = logger;
    }

    public Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        ServiceBusReceivedMessage? message = args.Message;
        string body = message.Body.ToString();
        _logger.LogInformation("Company Queue Listener Message: {Message}", body);

        return Task.CompletedTask;
    }

    public Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "An unexpected error has occurred in the company queue listener.");

        return Task.CompletedTask;
    }
}
