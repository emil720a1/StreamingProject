using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Common;
using Shared.Extensions;
using StreamingProject.Contracts.Streams;
using StreamingProject.Domain;

namespace StreamingProject.Application.Service;

public class StreamService : IStreamService
{
    private readonly IStreamRepository _streamRepository;
    private readonly ILogger<StreamService> _logger;
    private readonly IValidator<CreateStreamDto> _createStreamDtoValidator;
    private readonly IMapper _mapper;
    
    
    public StreamService(IStreamRepository streamRepository, ILogger<StreamService> logger, IValidator<CreateStreamDto> createStreamDtoValidator, IMapper mapper)
    {
        _streamRepository = streamRepository;
        _logger = logger;
        _createStreamDtoValidator = createStreamDtoValidator;
        _mapper = mapper;
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

    public Task<StreamDetailsDto> JoinStreamAsync(Guid streamId, Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}