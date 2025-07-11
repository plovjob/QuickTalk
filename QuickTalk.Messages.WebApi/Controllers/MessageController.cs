using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickTalk.Messages.Application.Commands.SendMessage;
using QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;

namespace QuickTalk.Messages.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
internal class MessageController(ISender mediator) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> GetAllMessages()
    {
        return Ok(await mediator.Send(new GetAllMessagesAsyncRequest()));
    }

    [HttpPost("Send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageAsyncCommand command)
    {
        await mediator.Send(command);
        return StatusCode(204);
    }
}
