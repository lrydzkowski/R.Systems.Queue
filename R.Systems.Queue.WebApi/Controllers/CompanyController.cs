using MediatR;
using Microsoft.AspNetCore.Mvc;
using R.Systems.Queue.Core.Commands.SendCompanyToQueue;
using R.Systems.Queue.Core.Commands.SendCompanyToTopic;
using R.Systems.Queue.Core.Common.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace R.Systems.Queue.WebApi.Controllers;

[ApiController]
[Route("company")]
public class CompanyController : ControllerBase
{
    private readonly ISender _mediator;

    public CompanyController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("queue")]
    [SwaggerOperation(Summary = "Send a message with company to a queue")]
    [SwaggerResponse(statusCode: 204)]
    public async Task<IActionResult> SendCompanyToQueue(Company company)
    {
        SendCompanyToQueueCommand command = new(company);
        await _mediator.Send(command);

        return new NoContentResult();
    }

    [HttpPost]
    [Route("topic")]
    [SwaggerOperation(Summary = "Send a message with company to a topic")]
    [SwaggerResponse(statusCode: 204)]
    public async Task<IActionResult> SendCompanyToTopic(Company company)
    {
        SendCompanyToTopicCommand command = new(company);
        await _mediator.Send(command);

        return new NoContentResult();
    }
}
