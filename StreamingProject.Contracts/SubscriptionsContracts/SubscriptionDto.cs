namespace StreamingProject.Contracts.SubscriptionsContracts;

public record SubscriptionDto(Guid Id, Guid FollowedId, Guid FollowerId, DateTime? SubscriptionAt);

