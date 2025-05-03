using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Options;

public class Company2QueueOptions : IQueueOptions
{
    public const string Position = "Company2Queue";

    public string ConnectionString { get; set; } = "";

    public string QueueName { get; init; } = "";

    public bool CreateQueueOnStartup { get; init; }

    public bool UseMachineNameAsQueueSuffix { get; init; }
}
