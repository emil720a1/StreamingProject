using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.Subscription.SubscriptionService;
using StreamingProject.Contracts.SubscriptionsContracts;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [Authorize(Policy = "Permission.Create")]
    [HttpPost("subscribe")]

    public async Task<IActionResult> Subscribe(
        [FromBody] SubscriptionDto request,
        CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.SubscribeAsync(request,cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [Authorize(Policy = "Permission.Delete")]
    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe(
        [FromBody] UnSubscribeDto request,
        CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.UnsubscribeAsync(request, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("getSubscriptions")]

    public async Task<IActionResult> GetSubscriptions(CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();
        
        var request = new GetSubscriptionsDto(userId);
        var result = await _subscriptionService.GetSubscriptionsAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
    
    
}