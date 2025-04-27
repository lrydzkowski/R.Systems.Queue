using R.Systems.Queue.Core.Commands.SendCompanyToTopic;
using R.Systems.Queue.Core.Common.Models;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Senders;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Senders;

internal class CompanyTopicSender : ICompanyTopicSender
{
    private readonly IServiceBusSender<CompanyTopicMessage> _topicSender;

    public CompanyTopicSender(
        IServiceBusSender<CompanyTopicMessage> topicSender
    )
    {
        _topicSender = topicSender;
    }

    public async Task SendAsync(Company company, CancellationToken cancellationToken = default)
    {
        CompanyTopicMessage companyMessage = new(company.Id, company.Name);
        await _topicSender.EnqueueAsync(companyMessage, cancellationToken: cancellationToken);
    }
}
