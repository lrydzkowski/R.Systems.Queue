using R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Options;

public class CompanyQueueOptions : IQueueOptions
{
    public const string Position = "CompanyQueue";

    public bool IsEnabled { get; init; }

    public string ConnectionString { get; set; } = "";

    public string QueueName { get; init; } = "";

    public bool CreateQueueOnStartup { get; init; }

    public bool DeleteQueueOnShutdown { get; init; }

    public bool UseMachineNameAsQueueSuffix { get; init; }

    public TimeSpan DuplicateDetectionHistoryTimeWindow { get; init; }
}
