using R.Systems.Queue.Core.Commands.SendCompanyToQueue;
using R.Systems.Queue.Core.Common.Models;
using R.Systems.Queue.Infrastructure.ServiceBus.Common.Senders;

namespace R.Systems.Queue.Infrastructure.ServiceBus.Senders;

internal class CompanyQueueSender : ICompanyQueueSender
{
    private readonly IServiceBusSender<Company> _queueSender;

    public CompanyQueueSender(IServiceBusSender<Company> queueSender)
    {
        _queueSender = queueSender;
    }

    public async Task SendAsync(Company company)
    {
        await _queueSender.EnqueueAsync(company);
    }
}
