using Microsoft.Extensions.Hosting;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

public class WorkerServiceBus : IHostedService, IAsyncDisposable
{
    private readonly IEnumerable<IServiceBusConsumer> _serviceBusConsumers;

    public WorkerServiceBus(IEnumerable<IServiceBusConsumer> serviceBusConsumers)
    {
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
}
