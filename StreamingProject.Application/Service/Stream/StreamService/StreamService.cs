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

namespace StreamingProject.Application.Service.Stream.StreamService;

public class StreamService : IStreamService
{
    private readonly IStreamRepository _streamRepository;
    private readonly IValidator<CreateStreamDto> _createStreamDtoValidator;
    private readonly IValidator<JoinStreamDto> _joinStreamDtoValidator;
    private readonly IValidator<GetStreamByIdDto> _getStreamByIdDtoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<StreamService> _logger;
    
    
    public StreamService(IStreamRepository streamRepository, ILogger<StreamService> logger, IValidator<CreateStreamDto> createStreamDtoValidator, IMapper mapper, IValidator<JoinStreamDto> joinStreamDtoValidator, IValidator<GetStreamByIdDto> getStreamByIdDtoValidator)
    {
        _streamRepository = streamRepository;
        _createStreamDtoValidator = createStreamDtoValidator;
        _joinStreamDtoValidator = joinStreamDtoValidator;
        _getStreamByIdDtoValidator = getStreamByIdDtoValidator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<StreamDetailsDto, Failure>> CreateStreamAsync(CreateStreamDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _createStreamDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        var stream = new StreamEntity 
        {
            UserId = request.UserId,
            Id = Guid.NewGuid(),
            ChatId = Guid.NewGuid(),
            LikeCount = 0,
            
            ChatMessages = new List<ChatEntity>(),
            Likes = new List<StreamLikeEntity>(),
            
        };

        var savedStream = await _streamRepository.AddStreamAsync(stream);
        
        return _mapper.Map<StreamDetailsDto>(savedStream);
    }

    public async Task<Result<StreamDetailsDto, Failure>> JoinStreamAsync(JoinStreamDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _joinStreamDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        var streamToJoin = await _streamRepository.GetStreamByIdAsync(request.StreamId);

        if (streamToJoin == null)
        {
            var error = Error.Validation("StreamNotFound", "Stream not found", "StreamId");

            return Failure.FromError(error);
        }

        if (DateTime.UtcNow > streamToJoin.EndTime)
        {
            return Failure.FromError(Error.Validation("StreamEnded", "Stream ended", "StreamId"));            
        }
        
        _logger.LogInformation("User joined stream");
        
        var detailsDto = _mapper.Map<StreamDetailsDto>(streamToJoin);
        return Result.Success<StreamDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<StreamDetailsDto, Failure>> GetStreamByIdAsync(GetStreamByIdDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _getStreamByIdDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        var stream = await _streamRepository.GetStreamByIdAsync(request.streamId);

        if (stream == null)
        {
            var error = Error.Validation("StreamNotFound", "Stream not found", "StreamId");

            return Failure.FromError(error);
        }
        
        
        var detailsDto = _mapper.Map<StreamDetailsDto>(stream);
        return Result.Success<StreamDetailsDto, Failure>(detailsDto);
        
    }
}