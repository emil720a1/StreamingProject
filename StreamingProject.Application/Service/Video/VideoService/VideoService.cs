using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Service.Video.VideoRepository;
using StreamingProject.Contracts.VideoDto;
using StreamingProject.Contracts.VideoDto.Crud;
using StreamingProject.Domain.Video;

namespace StreamingProject.Application.Service.Video.VideoService;

public class VideoService : IVideoService
{
    private readonly IVideoRepository _videoRepository;
    private readonly IValidator<CreateVideoDto> _createVideoDtoValidator;
    private readonly IValidator<GetVideoDto> _getVideoDtoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<VideoService> _logger;


    public VideoService(
        IVideoRepository videoRepository,
        IValidator<CreateVideoDto> createVideoDtoValidator,
        IMapper mapper,
        ILogger<VideoService> logger,
        IValidator<GetVideoDto> getVideoDtoValidator)
    {
        _videoRepository = videoRepository;
        _createVideoDtoValidator = createVideoDtoValidator;
        _getVideoDtoValidator = getVideoDtoValidator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<VideoDetailsDto, Failure>> CreateVideoAsync(CreateVideoDto request, Guid userId,
        CancellationToken cancellationToken)
    {
        var validationResult = await _createVideoDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var video = VideoEntity.Create(
            request.Title,
            request.StreamId,
            userId,
            request.FileUrl,
            request.HlsUrl
        );

        var result = await _videoRepository.AddVideoAsync(video);
        _logger.LogInformation("Video {VideoId} created for user {UserId}", result.Id, userId);

        return _mapper.Map<VideoDetailsDto>(result);
    }

    public async Task<Result<VideoDetailsDto, Failure>> GetVideoAsync(GetVideoDto request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _getVideoDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

        if (video == null)
        {
            _logger.LogWarning("Video with ID {VideoId} not found", request.VideoId);
            return Failure.FromError(Error.NotFound("Video.NotFound", $"Video with ID {request.VideoId} was not found",
                Guid.Empty));
        }

        _logger.LogInformation("Video {VideoId} succesfully retrived", video.Id);

        var detailsDto = _mapper.Map<VideoDetailsDto>(video);

        return Result.Success<VideoDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<bool, Failure>> UpdateVideoAsync(UpdateVideoDto request, Guid userId,
        CancellationToken cancellationToken)
    {
        var video = await _videoRepository.GetVideoByIdAsync(request.Id);

        var accessResult = CheckVideoAccess(video, userId);
        if (accessResult.IsFailure) return accessResult.IsFailure;

        var videoEntity = accessResult.Value;
        videoEntity.Title = request.Title;

        await _videoRepository.UpdateVideoAsync(video);
        _logger.LogInformation("Video {VideoId} updated", video.Id);

        return true;
    }

    public async Task<Result<bool, Failure>> DeleteVideoAsync(DeleteVideoDto request, Guid userId,
        CancellationToken cancellationToken)
    {
        var video = await _videoRepository.GetVideoByIdAsync(request.Id);

        var accessResult = CheckVideoAccess(video, userId);
        if (accessResult.IsFailure) return accessResult.IsFailure;

        await _videoRepository.DeleteVideoAsync(video.Id);
        _logger.LogInformation("Video {videoId} deleted by owner", video.Id);

        return true;
    }

    private Result<VideoEntity, Failure> CheckVideoAccess(VideoEntity? video, Guid userId)
    {
        if (video == null)
            return Failure.FromError(Error.Validation("Video.NotFound", "Video not found"));

        if (video.UserId != userId)
            return Failure.FromError(Error.Validation("Video.Forbidden", "You don't have access to this video"));
        
        return Result.Success<VideoEntity, Failure>(video);
    }
}