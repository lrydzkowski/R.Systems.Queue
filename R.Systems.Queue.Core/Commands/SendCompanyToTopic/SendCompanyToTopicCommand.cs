using MediatR;
using R.Systems.Queue.Core.Common.Models;

namespace R.Systems.Queue.Core.Commands.SendCompanyToTopic;

public record SendCompanyToTopicCommand(Company Company) : IRequest;

public class SendCompanyToTopicCommandHandler : IRequestHandler<SendCompanyToTopicCommand>
{
    private readonly ICompanyTopicSender _sender;

    public SendCompanyToTopicCommandHandler(ICompanyTopicSender sender)
    {
        _sender = sender;
    }

    public async Task Handle(SendCompanyToTopicCommand command, CancellationToken cancellationToken)
    {
        await _sender.SendAsync(command.Company, cancellationToken);
    }
}
