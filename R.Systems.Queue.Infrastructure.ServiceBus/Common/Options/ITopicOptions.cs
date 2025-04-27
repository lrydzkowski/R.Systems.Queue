namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

public interface ITopicOptions : IServiceBusOptions
{
    string TopicName { get; init; }

    string SubscriptionName { get; init; }
}
