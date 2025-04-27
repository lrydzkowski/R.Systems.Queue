namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

public interface IQueueOptions : IServiceBusOptions
{
    string QueueName { get; init; }

    bool CreateQueueOnStartup { get; init; }

    bool UseMachineNameAsQueueSuffix { get; init; }
}
