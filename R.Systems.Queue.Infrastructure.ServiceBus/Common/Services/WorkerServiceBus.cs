using Microsoft.Extensions.Hosting;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Listeners;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

public class WorkerServiceBus : IHostedService, IAsyncDisposable
{
    public WorkerServiceBus(IEnumerable<IServiceBusListener> serviceBusListener)
    {
        ServiceBusListener = serviceBusListener;
    }

    private IEnumerable<IServiceBusListener> ServiceBusListener { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (IServiceBusListener serviceBusListener in ServiceBusListener)
        {
            await serviceBusListener.StartProcessingAsync();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (IServiceBusListener serviceBusListener in ServiceBusListener)
        {
            await serviceBusListener.StopProcessingAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (IServiceBusListener serviceBusListener in ServiceBusListener)
        {
            await serviceBusListener.DisposeAsync();
        }
    }
}
