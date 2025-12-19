using Microsoft.EntityFrameworkCore;
using StreamingProject.Application.Service.Chat.ChatRepository;
using StreamingProject.Domain.Chat;

namespace StreamingProject.Repository.Repositories.ChatRepositories;

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

    public Task<ChatEntity> GetChatMessageById(Guid messageId)
    {
        return _dbContext.ChatMessages.FirstOrDefaultAsync(a => a.Id == messageId);
    }

    public async Task<bool> UpdateChatMessageAsync(ChatEntity message)
    {
        _dbContext.ChatMessages.Update(message);
       var result = await _dbContext.SaveChangesAsync();

       return result > 0;
    }

    public async Task<bool> DeleteChatMessageAsync(Guid messageId)
    {
        if (messageId == Guid.Empty) return false;

        var result =  await _dbContext.ChatMessages
            .Where(a => a.Id == messageId)
            .ExecuteDeleteAsync();
         
         return result > 0;
    }
}