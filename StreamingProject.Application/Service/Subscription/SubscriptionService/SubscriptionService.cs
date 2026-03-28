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
    private readonly IValidator<UnSubscribeDto> _unsubscribeDtoValidator;
    private readonly IValidator<GetSubscriptionDto> _getSubscriptionDtoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository, IValidator<SubscriptionDto> subscribeDtoValidator, IValidator<UnSubscribeDto> unsubscribeDtoValidator, IValidator<GetSubscriptionDto> getSubscriptionDtoValidator, IMapper mapper, ILogger<SubscriptionService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscribeDtoValidator = subscribeDtoValidator;
        _unsubscribeDtoValidator = unsubscribeDtoValidator;
        _getSubscriptionDtoValidator = getSubscriptionDtoValidator;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<SubscriptionDetailsDto, Failure>> SubscribeAsync(SubscriptionDto request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _subscribeDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var subscription = new SubscriptionEntity
        {
            Id = request.Id,
            FollowedId = request.FollowedId,
            FollowerId = request.FollowerId,
            SubscriptionAt = request.SubscriptionAt ?? DateTime.UtcNow
        };

        var subscribe = await _subscriptionRepository.SubscribeAsync(subscription);

        if (subscribe == null)
        {
            return Failure.FromError(Error.Validation("SubscriptionNotFound", "Subscription not found", "SubscriptionId"));
        }
        
        _logger.LogInformation("Subscription created");
        
        var detailsDto = _mapper.Map<SubscriptionDetailsDto>(subscribe);
        return Result.Success < SubscriptionDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<SubscriptionDetailsDto, Failure>> UnsubscribeAsync(UnSubscribeDto request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _unsubscribeDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var result = await _subscriptionRepository.UnsubscribeAsync(request.FollowerId, request.FollowedId);

        if (result <= 0)
        {
            return Failure.FromError(Error.Validation("SubscriptionNotFound", "Subscription not found",
                "SubscriptionId"));
        }

        _logger.LogInformation("Subscription deleted");

        var detailsDto = _mapper.Map<SubscriptionDetailsDto>(null);
        
        return Result.Success<SubscriptionDetailsDto, Failure>(detailsDto);

    }

    public async Task<Result<List<SubscriptionDetailsDto>, Failure>> GetSubscriptionsAsync(GetSubscriptionDto request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _getSubscriptionDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var subscriptions = await _subscriptionRepository.GetSubscriptionsAsync(request.FollowerId);

        if (subscriptions == null)
        {
            return Failure.FromError(Error.Validation("SubscriptionsNotFound", "Subscriptions not found",
                "FollowerId"));
        }

        _logger.LogInformation("Get subscription");

        var detailsDto = _mapper.Map<List<SubscriptionDetailsDto>>(subscriptions);

        
        return Result.Success<List<SubscriptionDetailsDto>, Failure>(detailsDto);
    }
}