using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.VideoDto;
using StreamingProject.Contracts.VideoDto.Crud;

namespace StreamingProject.Application.Service.Video;

public interface IVideoService
{
    Task<Result<VideoDetailsDto, Failure>> CreateVideoAsync(CreateVideoDto request,Guid userId, CancellationToken cancellationToken);
    
    Task<Result<VideoDetailsDto, Failure>> GetVideoAsync(GetVideoDto request, CancellationToken cancellationToken);
    
    Task<Result<bool, Failure>> UpdateVideoAsync(UpdateVideoDto request, Guid userId, CancellationToken cancellationToken);
    
    Task<Result<bool, Failure>> DeleteVideoAsync(DeleteVideoDto request, Guid userId, CancellationToken cancellationToken);
}