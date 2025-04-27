using MediatR;
using R.Systems.Queue.Core.Common.Models;

namespace R.Systems.Queue.Core.Commands.SendCompanyToQueue;

public record SendCompanyToQueueCommand(Company Company) : IRequest;

public class SendCompanyToQueueCommandHandler : IRequestHandler<SendCompanyToQueueCommand>
{
    private readonly ICompanyQueueSender _sender;

    public SendCompanyToQueueCommandHandler(ICompanyQueueSender sender)
    {
        _sender = sender;
    }

    public async Task Handle(SendCompanyToQueueCommand command, CancellationToken cancellationToken)
    {
        await _sender.SendAsync(command.Company, cancellationToken);
    }
}
