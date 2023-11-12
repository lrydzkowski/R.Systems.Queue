namespace R.Systems.Queue.Infrastructure.ServiceBus.Options;

internal class ServiceBusOptions
{
    public const string Position = "ServiceBus";

    public CompanyQueueOptions CompanyQueue { get; init; } = new();

    public CompanyTopicOptions CompanyTopic { get; init; } = new();
}
