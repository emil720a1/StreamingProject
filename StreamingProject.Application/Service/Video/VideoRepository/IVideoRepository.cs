using StreamingProject.Domain.Video;

namespace StreamingProject.Application.Service.Video;

public interface IVideoRepository
{
    Task<VideoEntity> AddVideoAsync(VideoEntity video);
    
    Task<VideoEntity> GetVideoByIdAsync(Guid id);
    
    Task<List<VideoEntity>> GetVideosByUserIdAsync(Guid userId);
    
    Task<VideoEntity> GetVideoByStreamIdAsync(Guid streamId);
    Task<VideoEntity> UpdateVideoAsync(VideoEntity video);
    
    Task<bool> DeleteVideoAsync(Guid id);
    
}