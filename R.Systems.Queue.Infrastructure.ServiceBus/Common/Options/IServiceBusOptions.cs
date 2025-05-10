namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Options;

public interface IServiceBusOptions
{
    bool IsEnabled { get; init; }

    string ConnectionString { get; set; }
}
