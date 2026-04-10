using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.Video.VideoService;
using StreamingProject.Contracts.VideoDto.Crud;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoController(IVideoService videoService, ICurrentUser currentUser) : ApiControllerBase
{
    [Authorize(Policy = "Permission.Create")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateVideo(
        [FromBody] CreateVideoDto request,
        CancellationToken cancellationToken)
    {
        var video = await videoService.CreateVideoAsync(request, currentUser.Id, cancellationToken);
        return HandleResult(video);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("info")]
    public async Task<IActionResult> GetVideo(
        [FromQuery] GetVideoDto request,
        CancellationToken cancellationToken)
    {
        var video = await videoService.GetVideoAsync(request, cancellationToken);
        return HandleResult(video);
    }

    [Authorize(Policy = "Permission.Update")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateVideo(
        [FromBody] UpdateVideoDto request,
        CancellationToken cancellationToken)
    {
        var videoToUpdate = await videoService.UpdateVideoAsync(request, currentUser.Id, cancellationToken);
        return HandleResult(videoToUpdate);
    }

    [Authorize(Policy = "Permission.Delete")]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteVideo(
        [FromBody] DeleteVideoDto request,
        CancellationToken cancellationToken)
    {
        var videoToDelete = await videoService.DeleteVideoAsync(request, currentUser.Id, cancellationToken);
        return HandleResult(videoToDelete);
    }
}