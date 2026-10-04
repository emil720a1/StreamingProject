using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Domain;
using StreamingProject.Domain.Stream;
using StreamingProject.Domain.Stream.UserStream;

namespace StreamingProject.Application.Service.Stream.StreamRepository;

public interface IStreamRepository
{
    // Stream persistence
    Task<StreamEntity> AddStreamAsync(StreamEntity stream);
    Task<StreamEntity> UpdateStreamAsync(StreamEntity stream);

    // Stream queries
    Task<StreamEntity?> GetStreamByIdAsync(Guid id);
    Task<StreamEntity?> GetActiveStream(Guid userId);
    Task<List<StreamEntity>> GetAvailableStreamsAsync();
    Task<List<StreamEntity>> GetStreamsByUserId(Guid userId);

    // Participants
    Task<bool> HasJoinedStreamAsync(Guid streamId, Guid userId);
    Task<bool> AddParticipantAsync(UserStream userStream);
    Task<bool> RemoveParticipantAsync(Guid streamId, Guid userId);

    // Validation
    Task<bool> CheckStreamKeyExistsAsync(string streamKey);
}
