using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Service.Stream.StreamRepository;
using StreamingProject.Contracts.Streams;
using StreamingProject.Domain;
using StreamingProject.Domain.Chat;
using StreamingProject.Domain.Stream;
using StreamingProject.Domain.Stream.UserStream;

namespace StreamingProject.Application.Service.Stream.StreamService;

public class StreamService : IStreamService
{
    private readonly IStreamRepository _streamRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<StreamService> _logger;
    private readonly IValidator<CreateStreamDto> _createStreamDtoValidator;
    private readonly IValidator<JoinStreamDto> _joinStreamDtoValidator;
    private readonly Application.Interfaces.Hls.IHlsTranscoderService _hlsTranscoderService;
    
    
    public StreamService(
        IStreamRepository streamRepository,
        ILogger<StreamService> logger, 
        IValidator<CreateStreamDto> createStreamDtoValidator,
        IMapper mapper, 
        IValidator<JoinStreamDto> joinStreamDtoValidator,
        Application.Interfaces.Hls.IHlsTranscoderService hlsTranscoderService)
    {
        _streamRepository = streamRepository;
        _createStreamDtoValidator = createStreamDtoValidator;
        _joinStreamDtoValidator = joinStreamDtoValidator;
        _mapper = mapper;
        _logger = logger;
        _hlsTranscoderService = hlsTranscoderService;
    }

    public async Task<Result<StreamDetailsDto, Failure>> CreateStreamAsync(CreateStreamDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _createStreamDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();
        
        var stream = StreamEntity.Create(request.UserId);

        var savedStream = await _streamRepository.AddStreamAsync(stream);
        _logger.LogInformation("Stream {StreamId} created with key {StreamKey}", savedStream.Id, savedStream.StreamKey);
        
        var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "hls", savedStream.Id.ToString());
        await _hlsTranscoderService.StartTranscodingAsync(savedStream.StreamKey, outputDirectory, cancellationToken);
        
        return _mapper.Map<StreamDetailsDto>(savedStream);
    }

    public async Task<Result<StreamDetailsDto, Failure>> JoinStreamAsync(JoinStreamDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _joinStreamDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid) return validationResult.ToErrors();
        
        var stream = await _streamRepository.GetStreamByIdAsync(request.StreamId);

        if (stream == null)
            return Failure.FromError(Error.NotFound("Stream.NotFound", "Stream not found", null));

        if (stream.EndTime.HasValue)
            return Failure.FromError(Error.Validation("Stream.EndTime", "This stream has already ended", null));
        
        var alreadyJoined = await _streamRepository.HasJoinedStreamAsync(request.StreamId, request.UserId);

        if (alreadyJoined)
            return Failure.FromError(Error.Conflict("AlreadyJoined", "User already joined"));

        var participant = UserStream.Create(request.UserId, request.StreamId);
        
        var result = await _streamRepository.AddParticipantAsync(participant);
        if (result == null)
            return Failure.FromError(Error.Internal("Db.Error", "Failed to join stream"));

        _logger.LogInformation("User {UserId} joined stream {StreamId}", request.UserId, request.StreamId);
      
        return _mapper.Map<StreamDetailsDto>(stream);
    }

    public async Task<Result<StreamDetailsDto, Failure>> GetStreamByIdAsync(GetStreamByIdDto request, CancellationToken cancellationToken)
    {
        var stream = await _streamRepository.GetStreamByIdAsync(request.streamId);

        if (stream == null)
            return Failure.FromError(Error.NotFound("Stream.NotFound", "Stream not found", null));
        
        return _mapper.Map<StreamDetailsDto>(stream);
    }

    public async Task<Result<bool, Failure>> ValidateStreamKeyAsync(string streamKey, CancellationToken cancellationToken)
    {
        var keyExists = await _streamRepository.CheckStreamKeyExistsAsync(streamKey);

        if (!keyExists)
        {
            _logger.LogWarning("Invalid stream key attempt: {Key}", streamKey);
            return Failure.FromError(Error.Unauthorized("StreamKey.Invalid", "Invalid or non-existent stream key"));
        }
        
        return true;
    }

    public async Task<Result<bool, Failure>> EndStreamAsync(EndStreamDto request, CancellationToken cancellationToken)
    {
        var stream = await _streamRepository.GetStreamByIdAsync(request.StreamId);

        if (stream == null)
            return Failure.FromError(Error.NotFound("Stream.NotFound", "Stream not found", null));
            
        if (stream.UserId != request.UserId)
            return Failure.FromError(Error.Unauthorized("Stream.Unauthorized", "Cannot end someone else's stream"));
            
        stream.EndStream();
        await _streamRepository.UpdateStreamAsync(stream);
        
        await _hlsTranscoderService.StopTranscodingAsync(stream.StreamKey, cancellationToken);
        
        return true;
    }
}