using ChatMessanger.Server.Data;
using ChatMessanger.Server.Interfaces;
using ChatMessanger.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatMessanger.Server.Services
{
    public class MessageService : IMessenger
    {
        private AppDbContext _db;
        public MessageService(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task SaveMessageAsync(Message message)
        {
            await _db.Messages.AddAsync(message);
            _db.SaveChanges();
        }

        public async Task<List<MessageDTO>> ShowAllMessages()
        {
            var allMessages = await _db.Messages.Select(m
                => new MessageDTO
                {
                    UserName = m.UserName,
                    Text = m.Text,
                    TimeOfSend = m.TimeOfSend
                }).ToListAsync();

            return allMessages;
        }
    }
}
