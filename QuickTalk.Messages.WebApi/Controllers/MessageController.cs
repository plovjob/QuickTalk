using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickTalk.Messages.Application.Commands.SendMessage;
using QuickTalk.Messages.Application.Queries.GetAllMessagesAsync;

namespace QuickTalk.Messages.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ISender _mediator;
        public MessageController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetAllMessages()
        {
            return Ok(_mediator.Send(new GetAllMessagesAsyncRequest()));
        }

        [HttpPost("Send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageAsyncCommand command)
        {
            await _mediator.Send(command);
            return StatusCode(201);
        }
    }
}
