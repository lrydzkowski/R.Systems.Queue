using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

internal class QueueInfrastructureManager<TOptions> : IInfrastructureManager
    where TOptions : class, IQueueOptions
{
    private readonly ILogger<IInfrastructureManager> _logger;
    private readonly INamesResolver _namesResolver;
    private readonly TOptions _options;

    public QueueInfrastructureManager(
        IOptions<TOptions> options,
        INamesResolver namesResolver,
        ILogger<QueueInfrastructureManager<TOptions>> logger
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
            await CreateQueueAsync(_options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Service Bus infrastructure");
        }
    }

    private async Task CreateQueueAsync(IQueueOptions queueOptions, CancellationToken cancellationToken)
    {
        if (!queueOptions.CreateQueueOnStartup)
        {
            return;
        }

        string queueName = _namesResolver.ResolveQueueName(queueOptions);

        _logger.LogInformation("Creating queue: {QueueName}", queueName);

        try
        {
            ServiceBusAdministrationClient client = new(queueOptions.ConnectionString);
            Response<bool>? queueExists = await client.QueueExistsAsync(queueName, cancellationToken);
            if (queueExists?.Value == true)
            {
                _logger.LogInformation("Queue already exists: {QueueName}", queueName);

                return;
            }

            await client.CreateQueueAsync(queueName, cancellationToken);
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
}
