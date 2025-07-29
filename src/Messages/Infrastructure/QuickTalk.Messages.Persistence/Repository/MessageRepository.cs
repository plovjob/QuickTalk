using Microsoft.EntityFrameworkCore;
using QuickTalk.Messages.Domain.Dto;
using QuickTalk.Messages.Domain.Entities;
using QuickTalk.Messages.Domain.Interfaces;

namespace QuickTalk.Messages.Persistence.Repository;

public sealed class MessageRepository(MessageDbContext messageDbContext) : IMessageRepository
{
    public async Task<OperationResult<MessageDto>> SendMessageAsync(MessageDto messageDto)
    {
        var message = new Message(
            messageDto.UserName,
            messageDto.Text,
            messageDto.SentAt ?? default);

        var addedMessage = await messageDbContext.Messages.AddAsync(message);

        if (addedMessage.Entity == null)
        {
            var innerMessage = $"Message from user {messageDto.UserName} not saved";
            return OperationResult<MessageDto>
                .Failure(InternalError.ValueNotAdded(innerMessage));
        }

        return OperationResult<MessageDto>.Success(messageDto);
    }

    public async Task<OperationResult<IReadOnlyCollection<MessageDto>>> GetAllMessagesAsync()
    {
        var messages = await messageDbContext.Messages.ToListAsync();
        if(messages == null)
        {
            var innerMessage = $"No messages in chat";
            return OperationResult<IReadOnlyCollection<MessageDto>>
                .Failure(InternalError.NoMessages(innerMessage));
        }

        var messageDtoCollection =
            messages.Select(m => new MessageDto(m.UserName, m.Text, m.SentAt)).ToList().AsReadOnly();
        return OperationResult<IReadOnlyCollection<MessageDto>>.Success(messageDtoCollection);
    }
}
