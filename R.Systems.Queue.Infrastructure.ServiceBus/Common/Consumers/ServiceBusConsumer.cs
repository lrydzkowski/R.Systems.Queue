using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Consumers;

public interface IServiceBusConsumer : IAsyncDisposable
{
    Task StartProcessingAsync(CancellationToken cancellationToken = default);

    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}

internal abstract class ServiceBusConsumer<TConsumer> : IServiceBusConsumer
    where TConsumer : class, IMessageConsumer
{
    private readonly ILogger<ServiceBusConsumer<TConsumer>> _logger;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private readonly IServiceProvider _serviceProvider;
    protected readonly ServiceBusProcessorOptions ProcessorOptions;
    protected readonly ServiceBusClient? ServiceBusClient;
    private ServiceBusProcessor? _processor;

    protected ServiceBusConsumer(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        string serviceBusClientName,
        ServiceBusProcessorOptions processorOptions,
        IServiceProvider serviceProvider,
        ILogger<ServiceBusConsumer<TConsumer>> logger
    )
    {
        ServiceBusClient = serviceBusClientFactory.CreateClient(serviceBusClientName);
        ProcessorOptions = processorOptions;
        _serviceProvider = serviceProvider;
        _logger = logger;
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
        if (_processor is null)
        {
            return;
        }

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
        await _processor.StartProcessingAsync(cancellationToken);
    }

    protected abstract ServiceBusProcessor? CreateProcessor();

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
        _processor.ProcessMessageAsync -= ProcessMessageAsync;
        _processor.ProcessErrorAsync -= ProcessErrorAsync;
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        TConsumer? consumer = scope.ServiceProvider.GetService<TConsumer>();
        if (consumer is null)
        {
            throw new InvalidOperationException($"Cannot resolve consumer: '{typeof(TConsumer).FullName}'");
        }

        IMessageSerializer? messageSerializer = scope.ServiceProvider.GetService<IMessageSerializer>();
        if (messageSerializer is null)
        {
            throw new InvalidOperationException(
                $"Cannot resolve message serializer: '{typeof(IMessageSerializer).FullName}'"
            );
        }

        await consumer.ProcessMessageAsync(args, messageSerializer);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "An unexpected error has occurred in Service Bus consumer");

        return Task.CompletedTask;
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

        if (ServiceBusClient is not null)
        {
            await ServiceBusClient.DisposeAsync();
        }
    }
}
