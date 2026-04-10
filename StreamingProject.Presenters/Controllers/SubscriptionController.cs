using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.Subscription.SubscriptionService;
using StreamingProject.Contracts.SubscriptionsContracts;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController(ISubscriptionService subscriptionService, ICurrentUser currentUser) : ApiControllerBase
{
    [Authorize(Policy = "Permission.Create")]
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(
        [FromBody] SubscriptionDto request,
        CancellationToken cancellationToken)
    {
        var result = await subscriptionService.SubscribeAsync(request, cancellationToken);
        return HandleResult(result);
    }

    [Authorize(Policy = "Permission.Delete")]
    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe(
        [FromBody] UnSubscribeDto request,
        CancellationToken cancellationToken)
    {
        var result = await subscriptionService.UnsubscribeAsync(request, cancellationToken);
        return HandleResult(result);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("getSubscriptions")]
    public async Task<IActionResult> GetSubscriptions(CancellationToken cancellationToken)
    {
        var request = new GetSubscriptionsDto(currentUser.Id);
        var result = await subscriptionService.GetSubscriptionsAsync(request, cancellationToken);
        
        return HandleResult(result);
    }
}