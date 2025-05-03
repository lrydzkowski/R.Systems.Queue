using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Options;

public class Company2TopicOptions : ITopicOptions
{
    public const string Position = "Company2Topic";

    public string ConnectionString { get; set; } = "";

    public string TopicName { get; init; } = "";

    public string SubscriptionName { get; init; } = "";

    public bool CreateTopicOnStartup { get; init; }

    public bool UseMachineNameAsTopicSuffix { get; init; }

    public bool CreateSubscriptionOnStartup { get; init; }

    public bool UseMachineNameAsSubscriptionSuffix { get; init; }
}
