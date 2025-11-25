namespace StreamingProject.Contracts.SubscriptionsContracts;

public record SubscriptionDetailsDto(Guid Id, Guid FollowedId, Guid FollowerId, DateTime? SubscriptionAt);

