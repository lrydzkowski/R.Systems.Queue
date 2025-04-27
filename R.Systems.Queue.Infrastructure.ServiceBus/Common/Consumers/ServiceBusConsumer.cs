using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

public interface IServiceBusConsumer : IAsyncDisposable
{
    Task StartProcessingAsync(CancellationToken cancellationToken = default);

    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}

internal abstract class ServiceBusConsumer<TListener> : IServiceBusConsumer
    where TListener : class, IMessageConsumer
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    protected readonly ServiceBusClient ServiceBusClient;
    protected readonly ServiceBusProcessorOptions ProcessorOptions;

    private readonly TListener _listener;
    private ServiceBusProcessor? _processor;

    protected ServiceBusConsumer(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        TListener listener,
        string serviceBusClientName,
        ServiceBusProcessorOptions processorOptions
    )
    {
        ServiceBusClient = serviceBusClientFactory.CreateClient(serviceBusClientName);
        _listener = listener;
        ProcessorOptions = processorOptions;
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            await StartAsync(cancellationToken);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task StopProcessingAsync(CancellationToken cancellationToken)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            await CloseAsync(cancellationToken);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            await DisposeServiceBusAndProcessorAsync();
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (!CanBeStarted())
        {
            return;
        }

        _processor = CreateProcessor();
        _processor.ProcessMessageAsync += _listener.ProcessMessageAsync;
        _processor.ProcessErrorAsync += _listener.ProcessErrorAsync;
        await _processor.StartProcessingAsync(cancellationToken);
    }

    protected abstract ServiceBusProcessor CreateProcessor();

    private bool CanBeStarted()
    {
        return _processor is not { IsProcessing: true };
    }

    private async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        if (!CanBeClosed())
        {
            return;
        }

        await _processor!.CloseAsync(cancellationToken);
        _processor.ProcessMessageAsync -= _listener.ProcessMessageAsync;
        _processor.ProcessErrorAsync -= _listener.ProcessErrorAsync;
    }

    private bool CanBeClosed()
    {
        return _processor is { IsClosed: false };
    }

    private async Task DisposeServiceBusAndProcessorAsync()
    {
        if (_processor is not null)
        {
            await CloseAsync();
            await _processor.DisposeAsync();
        }

        await ServiceBusClient.DisposeAsync();
    }
}
