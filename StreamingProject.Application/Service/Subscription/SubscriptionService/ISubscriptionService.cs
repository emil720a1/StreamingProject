using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.SubscriptionsContracts;

namespace StreamingProject.Application.Service.Subscription.SubscriptionService;

public interface ISubscriptionService
{
    Task<Result<SubscriptionDetailsDto, Failure>> SubscribeAsync(SubscriptionDto subscriptionDto, CancellationToken cancellationToken);
    
    Task<Result<bool, Failure>> UnsubscribeAsync(UnSubscribeDto unsubscribeDto, CancellationToken cancellationToken);

    Task<Result<List<SubscriptionDetailsDto>, Failure>> GetSubscriptionsAsync(GetSubscriptionsDto request, CancellationToken cancellationToken);

}