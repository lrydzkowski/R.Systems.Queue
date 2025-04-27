using R.Systems.Queue.Core.Common.Models;

namespace R.Systems.Queue.Core.Commands.SendCompanyToTopic;

public interface ICompanyTopicSender
{
    Task SendAsync(Company company, CancellationToken cancellationToken = default);
}
