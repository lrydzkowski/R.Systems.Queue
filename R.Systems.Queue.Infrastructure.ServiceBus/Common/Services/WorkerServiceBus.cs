using Microsoft.Extensions.Hosting;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

public class WorkerServiceBus : IHostedService, IAsyncDisposable
{
    private readonly IEnumerable<IQueueInfrastructureManager> _queueInfrastructureManagers;
    private readonly IEnumerable<IServiceBusConsumer> _serviceBusConsumers;
    private readonly IEnumerable<ITopicInfrastructureManager> _topicInfrastructureManagers;

    public WorkerServiceBus(
        IEnumerable<IQueueInfrastructureManager> queueInfrastructureManagers,
        IEnumerable<ITopicInfrastructureManager> topicInfrastructureManagers,
        IEnumerable<IServiceBusConsumer> serviceBusConsumers
    )
    {
        _queueInfrastructureManagers = queueInfrastructureManagers;
        _topicInfrastructureManagers = topicInfrastructureManagers;
        _serviceBusConsumers = serviceBusConsumers;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (IServiceBusConsumer serviceBusConsumer in _serviceBusConsumers)
        {
            await serviceBusConsumer.DisposeAsync();
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await CreateInfrastructureAsync(cancellationToken);

        foreach (IServiceBusConsumer serviceBusConsumer in _serviceBusConsumers)
        {
            await serviceBusConsumer.StartProcessingAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (IServiceBusConsumer serviceBusConsumer in _serviceBusConsumers)
        {
            await serviceBusConsumer.StopProcessingAsync(cancellationToken);
        }
    }

    private async Task CreateInfrastructureAsync(CancellationToken cancellationToken)
    {
        foreach (IQueueInfrastructureManager queueInfrastructureManager in _queueInfrastructureManagers)
        {
            await queueInfrastructureManager.CreateInfrastructureAsync(cancellationToken);
        }

        foreach (ITopicInfrastructureManager topicInfrastructureManager in _topicInfrastructureManagers)
        {
            await topicInfrastructureManager.CreateInfrastructureAsync(cancellationToken);
        }
    }
}
