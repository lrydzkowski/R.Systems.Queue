using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

internal class TopicSubscriptionInfrastructureManager<TOptions> : IInfrastructureManager
    where TOptions : class, ITopicOptions
{
    private readonly ServiceBusAdministrationClient? _adminClient;
    private readonly CreateSubscriptionOptions _createSubscriptionOptions;
    private readonly ILogger<IInfrastructureManager> _logger;
    private readonly TOptions _options;

    public TopicSubscriptionInfrastructureManager(
        IAzureClientFactory<ServiceBusAdministrationClient> clientFactory,
        string serviceBusClientName,
        CreateSubscriptionOptions createSubscriptionOptions,
        IOptions<TOptions> options,
        ILogger<TopicSubscriptionInfrastructureManager<TOptions>> logger
    )
    {
        _options = options.Value;
        _adminClient = _options.IsEnabled ? clientFactory.CreateClient(serviceBusClientName) : null;
        _createSubscriptionOptions = createSubscriptionOptions;
        _logger = logger;
    }

    public async Task CreateInfrastructureAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateSubscriptionAsync(_options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Service Bus infrastructure");
        }
    }

    public async Task DeleteInfrastructureAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DeleteSubscriptionAsync(_options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Service Bus infrastructure");
        }
    }

    private async Task CreateSubscriptionAsync(ITopicOptions topicOptions, CancellationToken cancellationToken)
    {
        if (!topicOptions.IsEnabled || !topicOptions.CreateSubscriptionOnStartup)
        {
            return;
        }

        string topicName = _createSubscriptionOptions.TopicName;
        string subscriptionName = _createSubscriptionOptions.SubscriptionName;

        _logger.LogInformation(
            "Creating subscription: {SubscriptionName} for topic: {TopicName}",
            subscriptionName,
            topicName
        );

        try
        {
            Response<bool>? subscriptionExists = await _adminClient!.SubscriptionExistsAsync(
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

            await _adminClient.CreateSubscriptionAsync(_createSubscriptionOptions, cancellationToken);
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

    private async Task DeleteSubscriptionAsync(ITopicOptions topicOptions, CancellationToken cancellationToken)
    {
        if (!topicOptions.IsEnabled || !topicOptions.DeleteSubscriptionOnShutdown)
        {
            return;
        }

        string topicName = _createSubscriptionOptions.TopicName;
        string subscriptionName = _createSubscriptionOptions.SubscriptionName;
        _logger.LogInformation(
            "Deleting subscription: {SubscriptionName} in topic {TopicName}",
            subscriptionName,
            topicName
        );

        try
        {
            await _adminClient!.DeleteSubscriptionAsync(topicName, subscriptionName, cancellationToken);
            _logger.LogInformation(
                "Subscription deleted: {SubscriptionName} in topic {TopicName}",
                subscriptionName,
                topicName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to delete subscription: {SubscriptionName} in topic {TopicName}",
                subscriptionName,
                topicName
            );
        }
    }
}
