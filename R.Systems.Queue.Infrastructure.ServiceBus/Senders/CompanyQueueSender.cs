using R.Systems.Queue.Core.Commands.SendCompanyToQueue;
using R.Systems.Queue.Core.Common.Models;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Senders;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Senders;

internal class CompanyQueueSender : ICompanyQueueSender
{
    private readonly IServiceBusSender<CompanyQueueMessage> _queueSender;

    public CompanyQueueSender(IServiceBusSender<CompanyQueueMessage> queueSender)
    {
        _queueSender = queueSender;
    }

    public async Task SendAsync(Company company, CancellationToken cancellationToken = default)
    {
        CompanyQueueMessage companyMessage = new(company.Id, company.Name);
        await _queueSender.EnqueueAsync(companyMessage, cancellationToken: cancellationToken);
    }
}
