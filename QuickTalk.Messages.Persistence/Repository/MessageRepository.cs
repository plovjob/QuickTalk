using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Dto;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickTalk.Messages.Persistence.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private MessageDbContext _messageDbContext;
        public MessageRepository(MessageDbContext messageDbContext)
        {
            _messageDbContext = messageDbContext;
        }

        public async Task<IList<Message>> GetAllMessagesAsync()
        {
            var allMessages = await _messageDbContext.Messages.Select(m
                => new MessageDto
                {
                    UserName = m.UserName,
                    Text = m.Text,
                    TimeOfSend = m.TimeOfSend
                }).ToListAsync();

            return allMessages;
        }

        //что то надо сделать с возвратом
        public async Task SendMessageAsync(Message message)
        {
            await _messageDbContext.Messages.AddAsync(message);
            _messageDbContext.SaveChanges();
        }
    }
}
