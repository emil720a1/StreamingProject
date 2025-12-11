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

    [AuthorizeCreate]
    [HttpPost]

    public async Task<IActionResult> CreateStream([FromBody] CreateStreamDto request, CancellationToken cancellationToken)
    {
        var result = await _streamService.CreateStreamAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [AuthorizeRead]
    [HttpPost("join")]

    public async Task<IActionResult> JoinStream([FromBody] JoinStreamDto request, CancellationToken cancellationToken)
    {
        var result = await _streamService.JoinStreamAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [AuthorizeRead]
    [HttpGet]
    public async Task<IActionResult> GetStreamById([FromQuery] GetStreamByIdDto request,
        CancellationToken cancellationToken)
    {
        var result = await _streamService.GetStreamByIdAsync(request, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
    
    
    
    
    
    
}