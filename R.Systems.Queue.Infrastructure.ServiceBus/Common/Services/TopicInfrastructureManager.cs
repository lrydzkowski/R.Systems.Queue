using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

internal class TopicInfrastructureManager<TOptions> : IInfrastructureManager
    where TOptions : class, ITopicOptions
{
    private readonly ServiceBusAdministrationClient? _adminClient;
    private readonly CreateTopicOptions _createTopicOptions;
    private readonly ILogger<IInfrastructureManager> _logger;
    private readonly TOptions _options;

    public TopicInfrastructureManager(
        IAzureClientFactory<ServiceBusAdministrationClient> clientFactory,
        string serviceBusClientName,
        CreateTopicOptions createTopicOptions,
        IOptions<TOptions> options,
        ILogger<TopicInfrastructureManager<TOptions>> logger
    )
    {
        _options = options.Value;
        _adminClient = _options.IsEnabled ? clientFactory.CreateClient(serviceBusClientName) : null;
        _createTopicOptions = createTopicOptions;
        _logger = logger;
    }

    public async Task CreateInfrastructureAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateTopicAsync(_options, cancellationToken);
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
            await DeleteTopicAsync(_options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Service Bus infrastructure");
        }
    }

    private async Task CreateTopicAsync(
        ITopicOptions topicOptions,
        CancellationToken cancellationToken
    )
    {
        if (!topicOptions.IsEnabled || !topicOptions.CreateTopicOnStartup)
        {
            return;
        }

        string topicName = _createTopicOptions.Name;
        _logger.LogInformation("Creating topic: {TopicName}", topicName);

        try
        {
            Response<bool>? topicExists =
                await _adminClient!.TopicExistsAsync(topicName, cancellationToken);
            if (topicExists?.Value == true)
            {
                _logger.LogInformation("Topic already exists: {TopicName}", topicName);

                return;
            }

            await _adminClient.CreateTopicAsync(_createTopicOptions, cancellationToken);
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

    private async Task DeleteTopicAsync(ITopicOptions topicOptions, CancellationToken cancellationToken)
    {
        if (!topicOptions.IsEnabled || !topicOptions.DeleteTopicOnShutdown)
        {
            return;
        }

        string topicName = _createTopicOptions.Name;
        _logger.LogInformation("Deleting topic: {TopicName}", topicName);

        try
        {
            await _adminClient!.DeleteTopicAsync(_createTopicOptions.Name, cancellationToken);
            _logger.LogInformation("Topic deleted: {TopicName}", topicName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete topic: {TopicName}", topicName);
        }
    }
}
