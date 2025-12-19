using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service;
using StreamingProject.Application.Service.Stream.StreamService;
using StreamingProject.Contracts.Streams;
using StreamingProject.Presenters.ResponseExtensions;
using StreamingProject.Repository.AuthorizeAttributes;

namespace StreamingProject.Presenters;

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
        if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }
        
        var request = new CreateStreamDto(userId);
        
        var result = await _streamService.CreateStreamAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpPost("join")]

    public async Task<IActionResult> JoinStream(
        [FromBody] JoinStreamDto streamDto,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }
        
        var request = new JoinStreamDto(
            userId, 
            streamDto.StreamId);
        
        var result = await _streamService.JoinStreamAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpGet("{streamId}")]
    [Authorize(Policy = "Permission.Read")]
    public async Task<IActionResult> GetStreamById(
        [FromRoute] Guid streamId,  
        CancellationToken cancellationToken)
    {
        
        if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }
        
        var request =  new GetStreamByIdDto(streamId, userId);
        
        var result = await _streamService.GetStreamByIdAsync(request, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
}