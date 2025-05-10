namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

public interface IQueueOptions : IServiceBusOptions
{
    string QueueName { get; init; }

    bool CreateQueueOnStartup { get; init; }

    bool DeleteQueueOnShutdown { get; init; }

    bool UseMachineNameAsQueueSuffix { get; init; }

    TimeSpan DuplicateDetectionHistoryTimeWindow { get; init; }
}
