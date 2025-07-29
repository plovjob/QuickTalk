using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickTalk.Messages.Application.Commands.SendMessage;
using QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;
using QuickTalk.Messages.Domain.Dto;

namespace QuickTalk.Messages.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessageController(ISender mediator) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IActionResult> GetAllMessages()
    {
        var result = await mediator.Send(new GetAllMessagesAsyncRequest());
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest();
    }

    [HttpPost("Send")]
    public async Task<IActionResult> SendMessage([FromBody] MessageDto message)
    {
        var command = new SendMessageAsyncCommand(message);
        var result = await mediator.Send(command);
        if (result.IsSuccess)
        {
            return StatusCode(204);
        }

        return BadRequest();
    }
}
