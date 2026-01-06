using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Domain;
using StreamingProject.Domain.Stream;
using StreamingProject.Domain.Stream.UserStream;

namespace StreamingProject.Application.Service.Stream.StreamRepository;

public interface IStreamRepository
{

    Task<StreamEntity> AddStreamAsync(StreamEntity stream);
    
    Task<StreamEntity> UpdateStreamAsync(StreamEntity stream);
    
    Task<StreamEntity?> GetStreamByIdAsync(Guid id);

    Task<StreamEntity?> GetActiveStream(Guid userId);
    
    Task<List<StreamEntity>> GetStreamsByUserId(Guid userId);
    Task<bool> HasJoinedStreamAsync(Guid StreamId, Guid UserId);


    Task<StreamEntity?> AddParticipantAsync(UserStream userStream);
    
    Task<bool> RemoveParticipantAsync(Guid StreamId, Guid UserId);
    
    Task<bool> CheckStreamKeyExistsAsync(string streamKey);
}