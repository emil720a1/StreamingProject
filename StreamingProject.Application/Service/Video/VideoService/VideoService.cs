using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Contracts.VideoDto;
using StreamingProject.Contracts.VideoDto.Crud;
using StreamingProject.Domain.Video;

namespace StreamingProject.Application.Service.Video;

public class VideoService : IVideoService
{
    private readonly IVideoRepository _videoRepository;
    private readonly IValidator<CreateVideoDto> _createVideoDtoValidator;
    private readonly IValidator<GetVideoDto> _getVideoDtoValidator;
    private readonly IValidator<UpdateVideoDto> _updateVideoDtoValidator;
    private readonly IValidator<DeleteVideoDto> _deleteVideoDtoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<VideoService> _logger;
    

    public VideoService(IVideoRepository videoRepository, IValidator<CreateVideoDto> createVideoDtoValidator, IValidator<GetVideoDto> getVideoDtoValidator, IValidator<UpdateVideoDto> updateVideoDtoValidator, IValidator<DeleteVideoDto> deleteVideoDtoValidator, IMapper mapper, ILogger<VideoService> logger)
    {
        _videoRepository = videoRepository;
        _createVideoDtoValidator = createVideoDtoValidator;
        _getVideoDtoValidator = getVideoDtoValidator;
        _updateVideoDtoValidator = updateVideoDtoValidator;
        _deleteVideoDtoValidator = deleteVideoDtoValidator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<VideoDetailsDto, Failure>> CreateVideoAsync(CreateVideoDto request, Guid userId, CancellationToken cancellationToken)
    {
        var validationResult = await _createVideoDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var video = VideoEntity.Create(
            request.Title,
            request.StreamId,
            userId,
            request.FileUrl,
            request.HlsUrl
            );

        if (video == null) return Failure.FromError(Error.Validation("VideoNotFound", "Video not found", "VideoId"));

        var result = await _videoRepository.AddVideoAsync(video);
        
        _logger.LogInformation("Video {VideoId} created for user {UserId}", result.Id, userId);
        
        var detailsDto = _mapper.Map<VideoDetailsDto>(result);
        
        return Result.Success<VideoDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<VideoDetailsDto, Failure>> GetVideoAsync(GetVideoDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _getVideoDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var result = await _videoRepository.GetVideoByIdAsync(request.VideoId);
        
        if (result == null) return Failure.FromError(Error.Validation("VideoNotFound", "Video not found", "VideoId"));

        var detailsDto = _mapper.Map<VideoDetailsDto>(result);
        
        _logger.LogInformation("Video retrieved");

        return Result.Success<VideoDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<bool, Failure>> UpdateVideoAsync(UpdateVideoDto request, Guid userId, CancellationToken cancellationToken)
    {
        var validationResult = await _updateVideoDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var video = await _videoRepository.GetVideoByIdAsync(request.Id);

        if (video == null) return Failure.FromError(Error.Validation("VideoNotFound", "Video not found", "VideoId"));
        
        if (video.UserId != userId) return Failure.FromError(Error.Unauthorized("Unauthorized", "Unauthorized"));
        
        video.Title = request.Title;
        
        var result = await _videoRepository.UpdateVideoAsync(video);
        
        _logger.LogInformation("Video updated");
        
        return Result.Success<bool, Failure>(result != null);
    }

    public async Task<Result<bool, Failure>> DeleteVideoAsync(DeleteVideoDto request, Guid userId, CancellationToken cancellationToken)
    {
        var validationResult = await _deleteVideoDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        var result = await _videoRepository.DeleteVideoAsync(request.Id);
        
        _logger.LogInformation("Video deleted");
        
        return Result.Success<bool, Failure>(result != null);
    }
}