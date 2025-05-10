using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

internal class QueueInfrastructureManager<TOptions> : IInfrastructureManager
    where TOptions : class, IQueueOptions
{
    private readonly ServiceBusAdministrationClient? _adminClient;
    private readonly CreateQueueOptions _createQueueOptions;
    private readonly ILogger<IInfrastructureManager> _logger;
    private readonly TOptions _options;

    public QueueInfrastructureManager(
        IAzureClientFactory<ServiceBusAdministrationClient> clientFactory,
        string serviceBusClientName,
        CreateQueueOptions createQueueOptions,
        IOptions<TOptions> options,
        ILogger<QueueInfrastructureManager<TOptions>> logger
    )
    {
        _options = options.Value;
        _adminClient = _options.IsEnabled ? clientFactory.CreateClient(serviceBusClientName) : null;
        _createQueueOptions = createQueueOptions;
        _logger = logger;
    }

    public async Task CreateInfrastructureAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateQueueAsync(_options, cancellationToken);
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
            await DeleteQueueAsync(_options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Service Bus infrastructure");
        }
    }

    private async Task CreateQueueAsync(IQueueOptions queueOptions, CancellationToken cancellationToken)
    {
        if (!queueOptions.IsEnabled || !queueOptions.CreateQueueOnStartup)
        {
            return;
        }

        string queueName = _createQueueOptions.Name;
        _logger.LogInformation("Creating queue: {QueueName}", queueName);

        try
        {
            Response<bool>? queueExists = await _adminClient!.QueueExistsAsync(queueName, cancellationToken);
            if (queueExists?.Value == true)
            {
                _logger.LogInformation("Queue already exists: {QueueName}", queueName);

                return;
            }

            await _adminClient.CreateQueueAsync(_createQueueOptions, cancellationToken);
            _logger.LogInformation("Queue created: {QueueName}", queueName);
        }
        catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            _logger.LogInformation("Queue already exists: {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create queue: {QueueName}", queueName);
        }
    }

    private async Task DeleteQueueAsync(IQueueOptions queueOptions, CancellationToken cancellationToken)
    {
        if (!queueOptions.IsEnabled || !queueOptions.DeleteQueueOnShutdown)
        {
            return;
        }

        string queueName = _createQueueOptions.Name;
        _logger.LogInformation("Deleting queue: {QueueName}", queueName);

        try
        {
            await _adminClient!.DeleteQueueAsync(queueName, cancellationToken);
            _logger.LogInformation("Queue deleted: {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete queue: {QueueName}", queueName);
        }
    }
}
