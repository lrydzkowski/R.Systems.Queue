using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Options;

public class CompanyTopicOptions : ITopicOptions
{
    public const string Position = "CompanyTopic";

    public string ConnectionString { get; set; } = "";

    public string TopicName { get; init; } = "";

    public string SubscriptionName { get; init; } = "";
}
