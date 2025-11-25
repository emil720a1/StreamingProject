using FluentValidation;
using StreamingProject.Contracts.SubscriptionsContracts;

namespace StreamingProject.Application.Service.Subscription.SubscriptionValidators;

public class SubscribeValidator : AbstractValidator<SubscriptionDto>
{
    public SubscribeValidator()
    {
       RuleFor(x => x.Id).NotEmpty();
       
       RuleFor(x => x.FollowedId).NotEmpty();
       
       RuleFor(x => x.FollowerId).NotEmpty();
       
       RuleFor(x => x.SubscriptionAt).NotEmpty();
        
    }
}