using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.Stream.StreamService;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamsController(
    IStreamService streamService, 
    ICurrentUser currentUser,
    Application.Interfaces.Hls.IHlsTranscoderService hlsTranscoderService) : ApiControllerBase
{
    [Authorize(Policy = "Permission.Create")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateStream(CancellationToken cancellationToken)
    {
        var request = new CreateStreamDto(currentUser.Id);
        var result = await streamService.CreateStreamAsync(request, cancellationToken);
        
        return HandleResult(result);
    }

    [Authorize(Policy = "Permission.Create")]
    [HttpPost("{streamId:guid}/end")]
    public async Task<IActionResult> EndStream(
        [FromRoute] Guid streamId,
        CancellationToken cancellationToken)
    {
        var request = new EndStreamDto(streamId, currentUser.Id);
        var result = await streamService.EndStreamAsync(request, cancellationToken);
        
        return HandleResult(result);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpPost("join")]
    public async Task<IActionResult> JoinStream(
        [FromBody] Guid streamId,
        CancellationToken cancellationToken)
    {
        var request = new JoinStreamDto(currentUser.Id, streamId);
        var result = await streamService.JoinStreamAsync(request, cancellationToken);
        
        return HandleResult(result);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("{streamId:guid}")]
    public async Task<IActionResult> GetStreamById(
        [FromRoute] Guid streamId,  
        CancellationToken cancellationToken)
    {
        var request = new GetStreamByIdDto(streamId, currentUser.Id);
        var result = await streamService.GetStreamByIdAsync(request, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{streamId:guid}/hls")]
    public async Task<IActionResult> GetHlsUrl(
        [FromRoute] Guid streamId,
        CancellationToken cancellationToken)
    {
        var result = await hlsTranscoderService.GetHlsPlaylistUrlAsync(streamId.ToString(), cancellationToken);
        return HandleResult(result);
    }
}