using FluentValidation;
using StreamingProject.Contracts.SubscriptionsContracts;

namespace StreamingProject.Application.Service.Subscription.SubscriptionValidators;

public class GetSubscribeValidator : AbstractValidator<GetSubscriptionDto>
{
    public GetSubscribeValidator()
    {
        RuleFor(x => x.FollowerId).NotEmpty();
    }
}