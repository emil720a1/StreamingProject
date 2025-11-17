using StreamingProject.Domain;

namespace StreamingProject.Application;

public interface IStreamRepository
{

    Task<StreamEntity> AddStreamAsync(StreamEntity stream);
    
    Task<StreamEntity> UpdateStreamAsync(StreamEntity stream);
    
    Task<StreamEntity> GetStreamByIdAsync(Guid id);

    Task<StreamEntity> GetActiveStream(Guid userId);
    
    Task<List<StreamEntity>> GetStreamsByUserId(Guid userId);
}