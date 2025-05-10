using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Options;

public class CompanyTopicOptions : ITopicOptions
{
    public const string Position = "CompanyTopic";

    public bool IsEnabled { get; init; }

    public string ConnectionString { get; set; } = "";

    public string TopicName { get; init; } = "";

    public string SubscriptionName { get; init; } = "";

    public bool CreateTopicOnStartup { get; init; }

    public bool DeleteTopicOnShutdown { get; init; }

    public bool UseMachineNameAsTopicSuffix { get; init; }

    public bool CreateSubscriptionOnStartup { get; init; }

    public bool DeleteSubscriptionOnShutdown { get; init; }

    public bool UseMachineNameAsSubscriptionSuffix { get; init; }

    public TimeSpan DuplicateDetectionHistoryTimeWindow { get; init; }
}
