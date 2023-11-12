using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Listeners;

public interface IServiceBusListener : IAsyncDisposable
{
    Task StartProcessingAsync();

    Task StopProcessingAsync();
}

internal abstract class ServiceBusListener<TListener> : IServiceBusListener
    where TListener : class, IMessageListener
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    protected readonly ServiceBusClient ServiceBusClient;
    protected readonly ServiceBusProcessorOptions ProcessorOptions;

    private readonly TListener _listener;
    private ServiceBusProcessor? _processor;

    protected ServiceBusListener(
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

    public async Task StartProcessingAsync()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            await StartAsync();
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task StopProcessingAsync()
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            await CloseAsync();
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

    private async Task StartAsync()
    {
        if (!CanBeStarted())
        {
            return;
        }

        _processor = CreateProcessor();
        _processor.ProcessMessageAsync += _listener.ProcessMessageAsync;
        _processor.ProcessErrorAsync += _listener.ProcessErrorAsync;
        await _processor.StartProcessingAsync();
    }

    protected abstract ServiceBusProcessor CreateProcessor();

    private bool CanBeStarted()
    {
        return _processor is not { IsProcessing: true };
    }

    private async Task CloseAsync()
    {
        if (!CanBeClosed())
        {
            return;
        }

        await _processor!.CloseAsync();
        _processor.ProcessMessageAsync -= _listener.ProcessMessageAsync;
        _processor.ProcessErrorAsync -= _listener.ProcessErrorAsync;
    }

    private bool CanBeClosed()
    {
        return _processor is { IsClosed: false };
    }

    private async Task DisposeServiceBusAndProcessorAsync()
    {
        if (_processor != null)
        {
            await CloseAsync();
            await _processor.DisposeAsync();
        }

        await ServiceBusClient.DisposeAsync();
    }
}
