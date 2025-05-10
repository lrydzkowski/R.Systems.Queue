using Microsoft.Extensions.Hosting;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

public class WorkerServiceBus : IHostedService, IAsyncDisposable
{
    private readonly IEnumerable<IInfrastructureManager> _infrastructureManagers;
    private readonly IEnumerable<IServiceBusConsumer> _serviceBusConsumers;

    public WorkerServiceBus(
        IEnumerable<IInfrastructureManager> infrastructureManagers,
        IEnumerable<IServiceBusConsumer> serviceBusConsumers
    )
    {
        _infrastructureManagers = infrastructureManagers;
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

        await DeleteInfrastructureAsync(cancellationToken);
    }

    private async Task CreateInfrastructureAsync(CancellationToken cancellationToken)
    {
        foreach (IInfrastructureManager infrastructureManager in _infrastructureManagers)
        {
            await infrastructureManager.CreateInfrastructureAsync(cancellationToken);
        }
    }

    private async Task DeleteInfrastructureAsync(CancellationToken cancellationToken)
    {
        foreach (IInfrastructureManager infrastructureManager in _infrastructureManagers)
        {
            await infrastructureManager.DeleteInfrastructureAsync(cancellationToken);
        }
    }
}
