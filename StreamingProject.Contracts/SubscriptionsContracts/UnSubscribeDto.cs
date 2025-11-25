namespace StreamingProject.Contracts.SubscriptionsContracts;

public record UnSubscribeDto(Guid FollowedId, Guid FollowerId);