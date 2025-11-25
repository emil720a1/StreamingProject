using Microsoft.EntityFrameworkCore;
using StreamingProject.Application;
using StreamingProject.Domain;

namespace StreamingProject.Repository.Repositories;

public class ChatRepositories : IChatRepository
{
    private readonly StreamingDbContext _dbContext;

    public ChatRepositories(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ChatEntity> SendMessageAsync(ChatEntity message)
    {

        await _dbContext.ChatMessages.AddAsync(message);
        await _dbContext.SaveChangesAsync();

        return message;
    }

    public async Task<List<ChatEntity>> GetChatMessagesAsync(Guid streamId)
    {
        return await _dbContext.ChatMessages
            .Where(a => a.StreamId == streamId)
            .ToListAsync();

    }

    public Task<bool> LeaveChatAsync(Guid streamId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteChatMessageAsync(Guid messageId)
    {
        if (messageId == Guid.Empty) return false;

         await _dbContext.ChatMessages
            .Where(a => a.Id == messageId)
            .ExecuteDeleteAsync();
         
         return true;
    }
}