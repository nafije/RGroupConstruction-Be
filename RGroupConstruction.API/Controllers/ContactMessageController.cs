using RGroupConstruction.API.Controllers.Base;
using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Features.ContactMessage.Commands.CreateContactMessage;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RGroupConstruction.API.Controllers;

public class ContactMessageController(IMediator _mediator, IMessageLocalizer localizer) : ApiControllerBase(_mediator, localizer)
{
    [HttpPost("contact")]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Subscribe([FromBody] CreateContactMessageCommand command)
    {
        return await SendCommand(command, _localizer[MessageKeys.Success.Contact.Created]);
    }

}

