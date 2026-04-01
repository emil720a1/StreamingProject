using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.Stream.StreamService;
using StreamingProject.Contracts.Streams;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]

public class StreamsController : ControllerBase
{
    private readonly IStreamService _streamService;

    public StreamsController(IStreamService streamService)
    {
        _streamService = streamService;
    }

    [Authorize(Policy = "Permission.Create")]
    [HttpPost("create")]

    public async Task<IActionResult> CreateStream(CancellationToken cancellationToken)
    {
        var userId = GetUsetId();
        if (userId == Guid.Empty) return Unauthorized("User ID not found in token.");
        
        var request = new CreateStreamDto(userId);
        var result = await _streamService.CreateStreamAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpPost("join")]

    public async Task<IActionResult> JoinStream(
        [FromBody] Guid streamId,
        CancellationToken cancellationToken)
    {
        var userId = GetUsetId();
        
        if (userId == Guid.Empty) return Unauthorized("User ID not found in token.");
        
        var request = new JoinStreamDto(userId, streamId);
        
        var result = await _streamService.JoinStreamAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("{streamId:guid}")]
    public async Task<IActionResult> GetStreamById(
        [FromRoute] Guid streamId,  
        CancellationToken cancellationToken)
    {

        var userId = GetUsetId();
        if (userId == Guid.Empty) return Unauthorized("User ID not found in token.");
        
        var request =  new GetStreamByIdDto(streamId, userId);
        
        var result = await _streamService.GetStreamByIdAsync(request, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    public Guid GetUsetId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        return (claim != null && Guid.TryParse(claim.Value, out var userId))
            ? userId
            : Guid.Empty;
    }
}