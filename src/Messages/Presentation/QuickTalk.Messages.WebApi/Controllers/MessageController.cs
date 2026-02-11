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
    public async Task<IActionResult> GetUserMessagesAsync(Guid senderId, Guid consumerId)
    {
        var result = await mediator.Send(new GetAllMessagesAsyncRequest(senderId, consumerId));

        if (!result.IsSuccess)
        {
            return BadRequest();
        }

        var response = result.Value.Select(m
            => new MessageOutputModel
            {
                Id = m.Id,
                Text = m.Text,
                FromUserId = senderId,
                ToUserId = consumerId,
                SentAt = m.SentAt ?? throw new ArgumentNullException(),
                EditedAt = m.EditedAt ?? null
            });

        return response == null ? NotFound() : Ok(response);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserChatPartners(Guid consumerId)
    {

    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> SendMessageAsync([FromBody] MessageInputModel messageParams)
    {
        var user = new MessengerUser(messageParams.FromUserId, messageParams.UserName);

        var message = new Message(
            messageParams.MessageId,
            messageParams.Text);

        var command = new SendMessageAsyncCommand(message, user);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest();
        }

        return NoContent();
    }
}
