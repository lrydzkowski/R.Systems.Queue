using R.Systems.Queue.Core.Commands.SendCompanyToTopic;
using R.Systems.Queue.Core.Common.Models;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Senders;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Senders;

internal class CompanyTopicSender : ICompanyTopicSender
{
    private readonly IServiceBusSender<Company> _topicSender;

    public CompanyTopicSender(
        IServiceBusSender<Company> topicSender
    )
    {
        _topicSender = topicSender;
    }

    public async Task SendAsync(Company company)
    {
        await _topicSender.EnqueueAsync(company);
    }
}
