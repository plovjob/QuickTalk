using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickTalk.Messages.Application.Commands.SendMessage;
using QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.WebApi.Dto;

namespace QuickTalk.Messages.WebApi.Controllers;

[Route("api/messages")]
[ApiController]
public class MessageController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllMessagesAsync()
    {
        var result = await mediator.Send(new GetAllMessagesAsyncRequest());
        if (!result.IsSuccess)
        {
            return BadRequest();
        }

        var response = result.Value.Select(m
            => new MessageOutputModel
            {
                Id = m.Id,
                UserName = m.UserName,
                Text = m.Text,
                SentAt = m.SentAt ?? throw new ArgumentNullException(),
                EditedAt = m.EditedAt ?? null
            });

        return response == null ? NotFound() : Ok(response);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> SendMessageAsync([FromBody] MessageInputModel messageParams)
    {
        var message = new Message(
            messageParams.Id,
            messageParams.UserName,
            messageParams.Text);

        var command = new SendMessageAsyncCommand(message);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest();
        }

        return NoContent();
    }
}
