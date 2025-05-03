using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

internal class TopicInfrastructureManager<TOptions> : IInfrastructureManager
    where TOptions : class, ITopicOptions
{
    private readonly ILogger<IInfrastructureManager> _logger;
    private readonly INamesResolver _namesResolver;
    private readonly TOptions _options;

    public TopicInfrastructureManager(
        IOptions<TOptions> options,
        INamesResolver namesResolver,
        ILogger<TopicInfrastructureManager<TOptions>> logger
    )
    {
        _options = options.Value;
        _namesResolver = namesResolver;
        _logger = logger;
    }

    public async Task CreateInfrastructureAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateTopicAsync(_options, cancellationToken);
            await CreateSubscriptionAsync(_options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Service Bus infrastructure");
        }
    }

    private async Task CreateTopicAsync(
        ITopicOptions topicOptions,
        CancellationToken cancellationToken
    )
    {
        if (!topicOptions.CreateTopicOnStartup)
        {
            return;
        }

        string topicName = _namesResolver.ResolveTopicName(topicOptions);

        _logger.LogInformation("Creating topic: {TopicName}", topicName);

        try
        {
            ServiceBusAdministrationClient client = new(topicOptions.ConnectionString);
            Response<bool>? topicExists = await client.TopicExistsAsync(topicName, cancellationToken);
            if (topicExists?.Value == true)
            {
                _logger.LogInformation("Topic already exists: {TopicName}", topicName);

                return;
            }

            await client.CreateTopicAsync(topicName, cancellationToken);
            _logger.LogInformation("Topic created: {TopicName}", topicName);
        }
        catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            _logger.LogInformation("Topic already exists: {TopicName}", topicName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create topic: {TopicName}", topicName);
        }
    }

    private async Task CreateSubscriptionAsync(ITopicOptions topicOptions, CancellationToken cancellationToken)
    {
        if (!topicOptions.CreateSubscriptionOnStartup)
        {
            return;
        }

        string topicName = _namesResolver.ResolveTopicName(topicOptions);
        string subscriptionName = _namesResolver.ResolveSubscriptionName(topicOptions);

        _logger.LogInformation(
            "Creating subscription: {SubscriptionName} for topic: {TopicName}",
            subscriptionName,
            topicName
        );

        try
        {
            ServiceBusAdministrationClient client = new(topicOptions.ConnectionString);
            Response<bool>? subscriptionExists = await client.SubscriptionExistsAsync(
                topicName,
                subscriptionName,
                cancellationToken
            );
            if (subscriptionExists?.Value == true)
            {
                _logger.LogInformation(
                    "Subscription already exists: {SubscriptionName} for topic: {TopicName}",
                    subscriptionName,
                    topicName
                );

                return;
            }

            await client.CreateSubscriptionAsync(topicName, subscriptionName, cancellationToken);
            _logger.LogInformation(
                "Subscription created: {SubscriptionName} for topic: {TopicName}",
                subscriptionName,
                topicName
            );
        }
        catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            _logger.LogInformation(
                "Subscription already exists: {SubscriptionName} for topic: {TopicName}",
                subscriptionName,
                topicName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create subscription: {SubscriptionName} for topic: {TopicName}",
                subscriptionName,
                topicName
            );
        }
    }
}
