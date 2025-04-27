using R.Systems.Queue.Core.Common.Models;

namespace R.Systems.Queue.Core.Commands.SendCompanyToQueue;

public interface ICompanyQueueSender
{
    Task SendAsync(Company company, CancellationToken cancellationToken = default);
}
