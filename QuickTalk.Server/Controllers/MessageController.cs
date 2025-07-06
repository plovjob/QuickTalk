using QuickTalk.Server.Interfaces;
using QuickTalk.Server.Models;
using QuickTalk.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuickTalk.Server.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessenger _messageService;
        public MessageController(IMessenger messageService)
        {
            _messageService = messageService;
        }

        [Route("msgs/showall")]
        [HttpGet]
        public async Task<ActionResult<List<MessageDTO>>> ShowAllMessages()
        {
            var content = await _messageService.ShowAllMessages();
            return Ok(content);
        }

        [Route("msgs/save")]
        [HttpPost]
        public IActionResult SaveMessage(MessageDTO messageDto)
        {
            var message = new Message()
            {
                UserName = messageDto.UserName,
                Text = messageDto.Text,
                //создавать самим время на сервере
                TimeOfSend = messageDto.TimeOfSend
            };


            _messageService.SaveMessageAsync(message);
            return Ok();
        }
    }
}
