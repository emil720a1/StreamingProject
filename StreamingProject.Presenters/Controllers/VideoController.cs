using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.Video.VideoService;
using StreamingProject.Contracts.VideoDto.Crud;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters.Controllers;


[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly IVideoService _videoService;

    public VideoController(IVideoService videoService)
    {
        _videoService = videoService;
    }

    [Authorize(Policy = "Permission.Create")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateVideo(
        [FromBody] CreateVideoDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();
        
        var video = await _videoService.CreateVideoAsync(request, userId, cancellationToken);
        return video.IsFailure ? video.Error.ToResponse() : Ok(video.Value);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("info")]
    public async Task<IActionResult> GetVideo(
        [FromQuery] GetVideoDto request,
        CancellationToken cancellationToken)
    {
        var video = await _videoService.GetVideoAsync(request, cancellationToken);
        return video.IsFailure ? video.Error.ToResponse() : Ok(video.Value);
    }

    [Authorize(Policy = "Permission.Update")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateVideo(
        [FromBody] UpdateVideoDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();
        
        var videoToUpdate = await _videoService.UpdateVideoAsync(request, userId, cancellationToken);
        return videoToUpdate.IsFailure ? videoToUpdate.Error.ToResponse() : Ok(videoToUpdate.Value);
    }

    [Authorize(Policy = "Permission.Delete")]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteVideo(
        [FromBody] DeleteVideoDto request,
        CancellationToken cancellationToken)
    {
        
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();
        
        var videoToDelete = await _videoService.DeleteVideoAsync(request, userId, cancellationToken);
        return videoToDelete.IsFailure ? videoToDelete.Error.ToResponse() : Ok(videoToDelete.Value);
    }

    private Guid GetUserId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        return (claim != null && Guid.TryParse(claim.Value, out var userId))
            ? userId
            : Guid.Empty;
    }
}