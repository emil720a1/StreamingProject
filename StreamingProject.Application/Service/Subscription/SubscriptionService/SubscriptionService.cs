using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Service.Subscription.SubscriptionRepository;
using StreamingProject.Contracts.SubscriptionsContracts;
using StreamingProject.Domain;
using StreamingProject.Domain.Subscription;

namespace StreamingProject.Application.Service.Subscription.SubscriptionService;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IValidator<SubscriptionDto> _subscribeDtoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository, 
        IValidator<SubscriptionDto> subscribeDtoValidator, 
        IMapper mapper,
        ILogger<SubscriptionService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscribeDtoValidator = subscribeDtoValidator;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<SubscriptionDetailsDto, Failure>> SubscribeAsync(SubscriptionDto request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _subscribeDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        try
        {
            var subscription = SubscriptionEntity.Create(request.FollowerId, request.FollowedId);
            
            var result = await _subscriptionRepository.SubscribeAsync(subscription, cancellationToken);
            _logger.LogInformation("User {FollowerId} followerd {FollowedId}", request.FollowerId, request.FollowedId);
            
            return _mapper.Map<SubscriptionDetailsDto>(result);
        }
        catch (InvalidOperationException ex)
        {
            return Failure.FromError(Error.Validation("Subscription.Self", ex.Message));
        }
    }

    public async Task<Result<bool, Failure>> UnsubscribeAsync(UnSubscribeDto request,
        CancellationToken cancellationToken)
    {
        var deleted = await _subscriptionRepository.UnsubscribeAsync(request.FollowerId, request.FollowedId);

        if (!deleted)
        {
            return Failure.FromError(Error.NotFound("Subscription.NotFound", "Subscription not found", null));
        }
        
        _logger.LogInformation("User {FollowerId} unfollowed {FollowedId}", request.FollowerId, request.FollowedId);
        return true;
    }

    public async Task<Result<List<SubscriptionDetailsDto>, Failure>> GetSubscriptionsAsync(GetSubscriptionsDto request, CancellationToken cancellationToken)
    {
        var subscriptions = await _subscriptionRepository.GetSubscriptionsAsync(request.FollowerId);
        var detailsDto = _mapper.Map<List<SubscriptionDetailsDto>>(subscriptions);
        
        return Result.Success<List<SubscriptionDetailsDto>, Failure>(detailsDto);
    }
}