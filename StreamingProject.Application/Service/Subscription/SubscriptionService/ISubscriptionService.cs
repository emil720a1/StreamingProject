using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.SubscriptionsContracts;

namespace StreamingProject.Application;

public interface ISubscriptionService
{
    Task<Result<SubscriptionDetailsDto, Failure>> SubscribeAsync(SubscriptionDto subscriptionDto, CancellationToken cancellationToken);
    
    Task<Result<SubscriptionDetailsDto, Failure>> UnsubscribeAsync(UnSubscribeDto unsubscribeDto, CancellationToken cancellationToken);

    Task<Result<List<SubscriptionDetailsDto>, Failure>> GetSubscriptionsAsync(GetSubscriptionDto getSubscriptionDto, CancellationToken cancellationToken);

}