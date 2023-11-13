using Azure.Messaging.ServiceBus;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Listeners;

namespace R.Systems.Queue.WebApi.Listeners;

public class CompanyTopicListener : IMessageListener
{
    private readonly ILogger<CompanyTopicListener> _logger;

    public CompanyTopicListener(ILogger<CompanyTopicListener> logger)
    {
        _logger = logger;
    }

    public Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        ServiceBusReceivedMessage? message = args.Message;
        string body = message.Body.ToString();
        _logger.LogInformation("Company Topic Listener Message: {Message}", body);

        return Task.CompletedTask;
    }

    public Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "An unexpected error has occurred in the company topic listener.");

        return Task.CompletedTask;
    }
}
