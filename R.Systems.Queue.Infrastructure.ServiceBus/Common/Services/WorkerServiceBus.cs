using Microsoft.Extensions.Hosting;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

public class WorkerServiceBus : IHostedService, IAsyncDisposable
{
    private readonly IEnumerable<IServiceBusQueueInfrastructureManager> _queueInfrastructureManagers;
    private readonly IEnumerable<IServiceBusConsumer> _serviceBusConsumers;
    private readonly IEnumerable<IServiceBusTopicInfrastructureManager> _topicInfrastructureManagers;

    public WorkerServiceBus(
        IEnumerable<IServiceBusQueueInfrastructureManager> queueInfrastructureManagers,
        IEnumerable<IServiceBusTopicInfrastructureManager> topicInfrastructureManagers,
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
        foreach (IServiceBusQueueInfrastructureManager queueInfrastructureManager in _queueInfrastructureManagers)
        {
            await queueInfrastructureManager.CreateInfrastructureAsync(cancellationToken);
        }

        foreach (IServiceBusTopicInfrastructureManager topicInfrastructureManager in _topicInfrastructureManagers)
        {
            await topicInfrastructureManager.CreateInfrastructureAsync(cancellationToken);
        }
    }
}
