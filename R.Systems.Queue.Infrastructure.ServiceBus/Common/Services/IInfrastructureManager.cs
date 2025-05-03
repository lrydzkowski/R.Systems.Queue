namespace R.Systems.Queue.Infrastructure.ServiceBus.Common.Services;

public interface IInfrastructureManager
{
    Task CreateInfrastructureAsync(CancellationToken cancellationToken = default);
}
