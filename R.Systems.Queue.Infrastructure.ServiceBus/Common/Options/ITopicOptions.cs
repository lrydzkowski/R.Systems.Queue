namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

public interface ITopicOptions : IServiceBusOptions
{
    string TopicName { get; init; }

    string SubscriptionName { get; init; }

    bool CreateTopicOnStartup { get; init; }

    bool DeleteTopicOnShutdown { get; init; }

    bool UseMachineNameAsTopicSuffix { get; init; }

    bool CreateSubscriptionOnStartup { get; init; }

    bool DeleteSubscriptionOnShutdown { get; init; }

    bool UseMachineNameAsSubscriptionSuffix { get; init; }

    TimeSpan DuplicateDetectionHistoryTimeWindow { get; init; }
}
