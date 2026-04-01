using Microsoft.EntityFrameworkCore;
using StreamingProject.Application.Service.Video;
using StreamingProject.Application.Service.Video.VideoRepository;
using StreamingProject.Domain.Video;

namespace StreamingProject.Repository.Repositories.VideoRepositories;

public class VideoRepositories : IVideoRepository
{

    private readonly StreamingDbContext _dbContext;
    public VideoRepositories(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<VideoEntity> AddVideoAsync(VideoEntity video)
    {
      if (video.Id == Guid.Empty) video.Id = Guid.NewGuid();
      
      await _dbContext.Videos.AddAsync(video);
      await _dbContext.SaveChangesAsync();

      return video;
    }

    public async Task<VideoEntity> GetVideoByIdAsync(Guid id)
    {
        return await _dbContext.Videos.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<VideoEntity>> GetVideosByUserIdAsync(Guid userId)
    {
        return await _dbContext.Videos
            .Where(a => a.UserId == userId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<VideoEntity> GetVideoByStreamIdAsync(Guid streamId)
    {
        return await _dbContext.Videos
            .FirstOrDefaultAsync(a => a.StreamId == streamId);
    }

    public async Task<VideoEntity> UpdateVideoAsync(VideoEntity video)
    { 
        _dbContext.Videos.Update(video);
        
        await _dbContext.SaveChangesAsync();
        
        return video;

    }

    public async Task<bool> DeleteVideoAsync(Guid id)
    {
        var result = await _dbContext.Videos
            .Where(a => a.Id == id)
            .ExecuteDeleteAsync();
        
        return result > 0;
        
    }
}