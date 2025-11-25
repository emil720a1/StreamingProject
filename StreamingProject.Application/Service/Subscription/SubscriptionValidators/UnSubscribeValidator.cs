using FluentValidation;
using StreamingProject.Contracts.SubscriptionsContracts;

namespace StreamingProject.Application.Service.Subscription.SubscriptionValidators;

public class UnSubscribeValidator : AbstractValidator<UnSubscribeDto>
{
    public UnSubscribeValidator()
    {
        RuleFor(x => x.FollowedId).NotEmpty();
        
        RuleFor(x => x.FollowerId).NotEmpty();
    }
}