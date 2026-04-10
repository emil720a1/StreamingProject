using Microsoft.EntityFrameworkCore;
using StreamingProject.Application.Service.Stream.StreamRepository;
using StreamingProject.Domain;
using StreamingProject.Domain.Stream;
using StreamingProject.Domain.Stream.UserStream;

namespace StreamingProject.Repository.Repositories.StreamRepositories;

public class StreamRepositories : IStreamRepository
{
    private readonly StreamingDbContext _dbContext;

    public StreamRepositories(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<StreamEntity> AddStreamAsync(StreamEntity stream)
    {
        await _dbContext.Streams.AddAsync(stream);
        await _dbContext.SaveChangesAsync();
        
        return stream;
    }

    public async Task<StreamEntity> UpdateStreamAsync(StreamEntity stream)
    {
        await _dbContext.SaveChangesAsync();
        return stream;
    }

    public async Task<StreamEntity?> GetStreamByIdAsync(Guid id)
    {
        return await _dbContext.Streams
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.VideoEntity)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<StreamEntity?> GetActiveStream(Guid userId)
    {
        return await _dbContext.Streams 
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.EndTime == null)
            .FirstOrDefaultAsync();
    }

    public async Task<List<StreamEntity>> GetStreamsByUserId(Guid userId)
    {
        
        return await _dbContext.Streams
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasJoinedStreamAsync(Guid streamId, Guid userId)
    {
        return await _dbContext.UserStreams
            .AsNoTracking()
            .AnyAsync(s => s.StreamId == streamId && s.UserId == userId);
    }

    public async Task<bool> AddParticipantAsync(UserStream userStream)
    {
        await _dbContext.UserStreams.AddAsync(userStream);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveParticipantAsync(Guid streamId, Guid userId)
    {
        if (streamId == Guid.Empty || userId == Guid.Empty) return false;

        var deletedCount =  await _dbContext.UserStreams
            .Where(s => s.StreamId == streamId && s.UserId == userId)
            .ExecuteDeleteAsync();

        return deletedCount > 0;
    }

    public async Task<bool> CheckStreamKeyExistsAsync(string streamKey)
    {
        return await _dbContext.Streams.AnyAsync(s => s.StreamKey == streamKey);
    }
}