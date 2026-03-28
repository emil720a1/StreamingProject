using AutoMapper;
using StreamingProject.Contracts.SubscriptionsContracts;
using StreamingProject.Domain.Subscription;

namespace Shared.Common;

public class SubscriptionMapper : Profile
{
    public SubscriptionMapper()
    {
        CreateMap<SubscriptionEntity, SubscriptionDetailsDto>()
            .ConstructUsing(s => new SubscriptionDetailsDto(
                s.Id,
                s.FollowedId,
                s.FollowerId,
                s.SubscriptionAt));
    }
}